using DnsBman.Models;

namespace DnsBman.Services
{
    public class BmanCustomerService
    {
        private readonly DomainService _domainService;
        private readonly RecordClientService _recordClientService;

        public BmanCustomerService(DomainService domainService, RecordClientService recordClientService)
        {
            _domainService = domainService;
            _recordClientService = recordClientService;
        }

        public async Task<Customer> AddCustomer(Customer customer)
        {
            Domain domainBmanIt = await _domainService.GetDomain("bbyu.it");
            HttpResponseMessage responseMessageBmanIt = await _recordClientService.PostRecord(domainBmanIt.Id, customer.Name, customer.ValueBmanIt);

            if (responseMessageBmanIt.IsSuccessStatusCode)
            {
                Record recordBmanIt = await responseMessageBmanIt.Content.ReadAsAsync<Record>();
                customer.IdRecordBmanIt = recordBmanIt.Id;

                if (string.IsNullOrWhiteSpace(customer.ValueBmanShop))
                {
                    customer.ValueBmanShop = customer.ValueBmanIt;
                    return customer;
                }
                /*
                Domain domainBmanShop = await _domainService.GetDomain("bman.shop");
                HttpResponseMessage responseMessageBmanShop = await _recordClientService.PostRecord(domainBmanShop.Id, customer.Name, customer.ValueBmanShop);

                if (responseMessageBmanShop.IsSuccessStatusCode)
                {
                    Record recordBmanShop = await responseMessageBmanShop.Content.ReadAsAsync<Record>();
                    customer.IdRecordBmanShop = recordBmanShop.Id;

                    return customer;
                }
                */

                else
                {
                    HttpResponseMessage response = await _recordClientService.DeleteRecord(customer.IdRecordBmanIt);
                }

                //throw new Exception($"Error: {responseMessageBmanShop.StatusCode}, {await responseMessageBmanShop.Content.ReadAsStringAsync()}");
            }

            throw new Exception($"Error: {responseMessageBmanIt.StatusCode}, {await responseMessageBmanIt.Content.ReadAsStringAsync()}");
        }

        public async Task<Customer> EditCustomer(Customer customer, Customer modifiedCustomer)
        {
            if ((modifiedCustomer.Name != null && modifiedCustomer.Name != customer.Name) || (modifiedCustomer.ValueBmanIt != null && modifiedCustomer.ValueBmanIt != customer.ValueBmanIt))
            {
                if (modifiedCustomer.Name == null)
                {
                    modifiedCustomer.Name = customer.Name;
                }
                else if (modifiedCustomer.ValueBmanIt == null)
                {
                    modifiedCustomer.ValueBmanIt = customer.ValueBmanIt;
                }

                HttpResponseMessage responseMessageBmanIt = await _recordClientService.PutRecord(customer.IdRecordBmanIt, modifiedCustomer.Name!, modifiedCustomer.ValueBmanIt!);

                if (!responseMessageBmanIt.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: {responseMessageBmanIt.StatusCode}, {await responseMessageBmanIt.Content.ReadAsStringAsync()}");
                }

                customer.Name = modifiedCustomer.Name;
                customer.ValueBmanIt = modifiedCustomer.ValueBmanIt;
            }

            if ((modifiedCustomer.Name != null && modifiedCustomer.Name != customer.Name) || (modifiedCustomer.ValueBmanShop != null && modifiedCustomer.ValueBmanShop != customer.ValueBmanShop))
            {
                if (modifiedCustomer.Name == null)
                {
                    modifiedCustomer.Name = customer.Name;
                }
                else if (modifiedCustomer.ValueBmanShop == null)
                {
                    modifiedCustomer.ValueBmanShop = customer.ValueBmanShop;
                }

                HttpResponseMessage responseMessageBmanShop = await _recordClientService.PutRecord(customer.IdRecordBmanShop, modifiedCustomer.Name!, modifiedCustomer.ValueBmanShop!);

                if (!responseMessageBmanShop.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: {responseMessageBmanShop.StatusCode}, {await responseMessageBmanShop.Content.ReadAsStringAsync()}");
                }

                customer.Name = modifiedCustomer.Name;
                customer.ValueBmanShop = modifiedCustomer.ValueBmanShop;
            }

            return customer;
        }

        public async Task<bool> IsCustomerEliminated(Customer customer)
        {
            HttpResponseMessage responseMessageBmanIt = await _recordClientService.DeleteRecord(customer.IdRecordBmanIt);

            if (responseMessageBmanIt.IsSuccessStatusCode)
            {
                return true;
                /*
                HttpResponseMessage responseMessageBmanShop = await _recordClientService.DeleteRecord(customer.IdRecordBmanShop);

                if (responseMessageBmanShop.IsSuccessStatusCode)
                {
                    return true;
                }

                throw new Exception($"Error: {responseMessageBmanShop.StatusCode}, {await responseMessageBmanShop.Content.ReadAsStringAsync()}");
                */
            }

            throw new Exception($"Error: {responseMessageBmanIt.StatusCode}, {await responseMessageBmanIt.Content.ReadAsStringAsync()}");
        }
    }
}
