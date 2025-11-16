using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CustomerRepository
    {
        private FuminiHotelManagementContext _db;
        public CustomerRepository()
        {
            _db = new FuminiHotelManagementContext();
        }
        public Customer? GetCustomer(string email, string password)
        {
            return _db.Customers.FirstOrDefault(x => x.EmailAddress == email && x.Password == password);
        }

        public List<Customer> GetCustomers()
        {
            return _db.Customers.ToList();
        }

        public Customer? GetCustomerById(int customerId)
        {
            return _db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public void UpdateCustomer(Customer customer)
        {
            _db.Customers.Update(customer);
            _db.SaveChanges();
        }
    }
}
