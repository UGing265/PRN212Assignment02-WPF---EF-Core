using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CustomerService
    {
        private CustomerRepository _customerRepository;
        public CustomerService()
        {
           _customerRepository = new CustomerRepository(); 
        }
        public Customer? GetCustomer(string email, string password)
        {
            return _customerRepository.GetCustomer(email, password);
        }

        public List<Customer> GetCustomers()
        {
            return _customerRepository.GetCustomers();
        }
    }
}
