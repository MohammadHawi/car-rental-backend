public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICarRepository _carRepository;

    public ContractService(IContractRepository contractRepository, ICustomerRepository customerRepository, ICarRepository carRepository)
    {
        _contractRepository = contractRepository;
        _customerRepository = customerRepository;
        _carRepository = carRepository;
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
        if (string.IsNullOrWhiteSpace(dto.Car))
            throw new ArgumentException("Car plate cannot be null or empty.", nameof(dto.Car));

        var car = await _contractRepository.GetCarByPlateAsync(dto.Car);
        if (car == null)
            throw new Exception("Car not found.");

        Customer? customer = null;

        // Check if we're creating a new customer
        if (!string.IsNullOrWhiteSpace(dto.FirstName) &&
            !string.IsNullOrWhiteSpace(dto.LastName) &&
            dto.NationalityId.HasValue &&
            !string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            // Create a new customer
            customer = new Customer
            {
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                NationalityId = dto.NationalityId.Value,
                PhoneNumber = dto.PhoneNumber
            };

            await _customerRepository.AddAsync(customer);
        }
        else if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            // Look for existing customer
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
            Status = 1, // active  2--> overdue 3--> closed (then return = 1)
            Returned = 0,
        };

        await _contractRepository.AddAsync(contract);
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
                ? (int)(contract.Price * (contract.CheckIn.Value.DayNumber - contract.CheckOut.Value.DayNumber) - contract.Paid ?? 0)
                : 0,
            


        };

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



}