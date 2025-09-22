public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICarRepository _carRepository;
    private readonly ITransactionRepository _transactionRepository;

    public ContractService(IContractRepository contractRepository, ICustomerRepository customerRepository, ICarRepository carRepository, ITransactionRepository transactionRepository)
    {
        _contractRepository = contractRepository;
        _customerRepository = customerRepository;
        _carRepository = carRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<(IEnumerable<ContractResponseDto> Items, int TotalCount)> GetAllContractsAsync(int pageNumber, int pageSize, string? searchQuery = null)
    {
        return await _contractRepository.GetAllAsync(pageNumber, pageSize, searchQuery);
    }

    public async Task<Contract> GetContractByIdAsync(int id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
        {
            throw new KeyNotFoundException("Contract not found.");
        }
        return contract;
    }
    //     public async Task<ContractResponseDto> GetContractByIdAsync(int id)
    // {
    //     var contract = await _contractRepository.GetByIdAsync(id);
    //     if (contract == null)
    //     {
    //         throw new KeyNotFoundException("Contract not found.");
    //     }

    //     var car = await _carRepository.GetByIdAsync(contract.CarId);
    //     var customer = await _customerRepository.GetByIdAsync(contract.Cid);

    //     return new ContractResponseDto
    //     {
    //         Id = contract.Id,
    //         CarPlate = car?.Plate.ToString(),
    //         CustomerName = $"{customer?.FirstName} {customer?.MiddleName} {customer?.LastName}".Trim(),
    //         Price = contract.Price,
    //         CheckIn = contract.CheckIn,
    //         CheckOut = contract.CheckOut,
    //         Deposit = contract.Deposit,
    //         Paid = contract.Paid,
    //         Status = contract.Status,
    //         Total = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
    //             ? (int)(contract.Price * (contract.CheckOut.Value.DayNumber - contract.CheckIn.Value.DayNumber))
    //             : 0,
    //         Balance = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
    //             ? (int)((contract.Price * (contract.CheckOut.Value.DayNumber - contract.CheckIn.Value.DayNumber)) - contract.Paid ?? 0)
    //             : 0,
    //     };
    // }


    public async Task AddContractAsync(Contract contract)
    {
        await _contractRepository.AddAsync(contract);
    }

    public async Task UpdateContractAsync(Contract contract)
    {
        await _contractRepository.UpdateAsync(contract);
    }

    public async Task DeleteContractAsync(int id)
    {
        await _contractRepository.DeleteAsync(id);
    }

    public async Task<ContractResponseDto> CreateContractFromDtoAsync(ContractRequestDto dto)
    {
        try
        {
            if (dto.Car == null)
                throw new ArgumentException("Car plate cannot be null or empty.", nameof(dto.Car));

            var car = await _contractRepository.GetCarByPlateAsync((int)dto.Car);
            if (car == null)
                throw new Exception("Car not found.");
            car.Status = 1;
            await _carRepository.UpdateAsync(car);

            Customer? customer = null;

            if (dto.Cid == null)
            {
                customer = new Customer
                {
                    FirstName = dto.FirstName,
                    MiddleName = dto.MiddleName,
                    LastName = dto.LastName,
                    NationalityId = dto.NationalityId.Value,
                    PhoneNumber = dto.PhoneNumber
                };

                await _customerRepository.AddAsync(customer);
                dto.Name = $"{dto.FirstName} {dto.MiddleName} {dto.LastName}".Trim();
            }
            else if (dto.Cid != null)
            {
                var nameParts = dto.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var first = nameParts.ElementAtOrDefault(0);
                var middle = nameParts.Length > 2 ? nameParts[1] : null;
                var last = nameParts.Length > 1 ? nameParts[^1] : null;

                customer = await _contractRepository.GetCustomerByFullNameAsync(first, middle, last);
                if (customer == null)
                    throw new Exception("Customer not found.");
            }
            else
            {
                throw new Exception("Customer data is incomplete.");
            }

            var contract = new Contract
            {
                CarId = car.Id,
                Cid = customer.Id,
                CheckOut = DateOnly.FromDateTime(dto.PickOut),
                CheckIn = DateOnly.FromDateTime(dto.DropIn),
                Price = dto.Price,
                Deposit = dto.Deposit,
                Paid = dto.Paid,
                Status = 1,
                Returned = 0,
            };

            await _contractRepository.AddAsync(contract);

            var transaction = new Transaction
            {
                Type = TransactionType.Income,
                Category = 1,
                Amount = (decimal)(dto.Paid ?? 0),
                Date = DateTime.UtcNow,
                Description = $"payment by {dto.Name} for car {dto.Car}",
                ContractId = contract.Id,
                CarId = car.Id,
            };

            await _transactionRepository.CreateTransactionAsync(transaction);

            return new ContractResponseDto
            {
                Id = contract.Id,
                CarPlate = car?.Plate.ToString(),
                CustomerName = $"{customer?.FirstName} {customer?.MiddleName} {customer?.LastName}".Trim(),
                Price = contract.Price,
                CheckIn = contract.CheckIn,
                CheckOut = contract.CheckOut,
                Deposit = contract.Deposit,
                Paid = contract.Paid,
                Status = contract.Status,
                Total = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
                    ? (int)(contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber))
                    : 0,
                Balance = (contract.CheckIn.HasValue && contract.CheckOut.HasValue)
                    ? (int)((contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber)) - contract.Paid ?? 0)
                    : 0,
            };
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to create contract.", ex);
        }
    }


    public async Task ReturnContract(int contractId, DateTime checkInDate)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null)
            throw new KeyNotFoundException("Contract not found.");

        contract.CheckIn = DateOnly.FromDateTime(checkInDate);
        contract.Returned = 1; // Mark as returned
        contract.Status = 3; // Mark as closed

        await _contractRepository.UpdateAsync(contract);
    }
    
    public async Task ExtendContract(int contractId, DateTime checkInDate)
{
    try
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null)
            throw new KeyNotFoundException("Contract not found.");

        // Ensure original CheckIn date exists
        if (contract.CheckIn == null)
            throw new InvalidOperationException("Contract's original CheckIn date is not set.");

        var originalCheckIn = contract.CheckIn.Value;
        var newCheckIn = DateOnly.FromDateTime(checkInDate);

        // Calculate the number of days extended (use DateTime to handle month/year properly)
        var originalCheckInDateTime = originalCheckIn.ToDateTime(TimeOnly.MinValue);
        var newCheckInDateTime = newCheckIn.ToDateTime(TimeOnly.MinValue);

        int daysExtended = (newCheckInDateTime - originalCheckInDateTime).Days;

        if (daysExtended <= 0)
            throw new InvalidOperationException("New check-in date must be after the original check-in date.");

        // Calculate the new amount
        decimal amount = (decimal)(contract.Price * daysExtended);

        // Fetch car
        var car = await _carRepository.GetByIdAsync(contract.CarId);
        if (car == null)
            throw new KeyNotFoundException("Car not found.");

        // Fetch customer
        var customer = await _customerRepository.GetByIdAsync(contract.Cid);
        if (customer == null)
            throw new KeyNotFoundException("Customer not found.");

        // Construct full name
        string fullName = $"{customer.FirstName} {customer.MiddleName ?? ""} {customer.LastName}".Trim();

        // Create transaction
        var transaction = new Transaction
        {
            Type = TransactionType.Income,
            Category = 3,
            Amount = amount,
            Date = DateTime.UtcNow,
            Description = $"Payment by {fullName} for car {car.Plate}",
            ContractId = contract.Id,
            CarId = car.Id,
        };

        await _transactionRepository.CreateTransactionAsync(transaction);

        // Update the contract's CheckIn date
        contract.CheckIn = newCheckIn;
        await _contractRepository.UpdateAsync(contract);
    }
    catch (Exception ex)
    {
        // Log the exception (replace with your logger if needed)
        Console.WriteLine($"Error extending contract: {ex.Message}");

        // Optionally, rethrow or handle accordingly
        throw;
    }
}





}