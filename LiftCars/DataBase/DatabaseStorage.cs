using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LiftCars.Model.ModelCustomers;

namespace LiftCars.DataBase
{
    internal class DatabaseStorage
    {
        private const string DB_NAME = "demo_local_db.db3";
        private readonly SQLiteAsyncConnection _connection;

        public DatabaseStorage()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<Customer>();
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _connection.Table<Customer>().ToListAsync();
        }

        public async Task<Customer> GetById(int id)
        {
            return await _connection.Table<Customer>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<string> Create(Customer customer)
        {
            var existingCustomer = await _connection.Table<Customer>()
                .Where(x => x.Email == customer.Email || x.Password == customer.Password)
                .FirstOrDefaultAsync();

            if (existingCustomer != null)
            {
                return "Customer with this email or mobile number already exists.";
            }

            await _connection.InsertAsync(customer);
            return "Customer registered successfully.";
        }

        public async Task Update(Customer customer)
        {
            await _connection.UpdateAsync(customer);
        }

        public async Task Delete(Customer customer)
        {
            await _connection.DeleteAsync(customer);
        }

        public async Task<Customer> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            return await _connection.Table<Customer>()
                .Where(c => c.Email == email && c.Password == password)
                .FirstOrDefaultAsync();
        }

    }
}
