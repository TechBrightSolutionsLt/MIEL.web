using System;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using MIEL.web.Repositories;

namespace MIEL.web.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        // Constructor injection
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void RegisterCustomer(UserRegistrationVM vm)
        {
            if (vm.Password != vm.ConfirmPassword)
                throw new Exception("Passwords do not match");

            var user = new userModel
            {
                RoleId = 2,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                MobileNumber = vm.MobileNumber,
                Gender = vm.Gender,
                Address = vm.Address,
                City = vm.City,
                Postcode = vm.Postcode,
                Password = vm.Password,
                CreatedDate = DateTime.Now
            };

            _userRepository.Insert(user);
        }
    }
}
