using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITI.gRPC.API.Protos;
using Microsoft.AspNetCore.Mvc;
namespace ITI.gRPC.API.Services
{
    public class ApiProductService
    {
        private readonly productService.productServiceClient  _client;

        public ApiProductService(productService.productServiceClient client)
        {
            _client = client;
        }

        public async Task<confirmMessage> GetProductByIdAsync(int id)
        {
            productID request = new productID
            {
                Id = id
            };

            return await _client.getProductByIdAsync(request);
        }

        public async Task<productServiceMessage> AddProductAsync(productForAdd product)
        {
            return await _client.AddProductAsync(product);
        }

        public async Task<productServiceMessage> UpdateProductAsync(product product)
        {
            return await _client.UpdateProductAsync(product);
        }

       public async Task<NumberOfInsertedProducts> AddManyProducts(List<productForAdd> products)
        {
            var call = _client.AddBulkProd();
            foreach (var product in products)
            {
                await call.RequestStream.WriteAsync(product);
            }

            await call.RequestStream.CompleteAsync();
            var response = await call.ResponseAsync;

            return new NumberOfInsertedProducts() { NumOfInsertedRows = response.NumOfInsertedRows };
        }


        public async Task<List<product>> GetFilterdSortedProducts (ProductCriteriaMsg productCriteria)
        {
            var productList = new List<product>();

            using (var call = _client.GetProductsByCriteria(productCriteria))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    productList.Add(call.ResponseStream.Current);
                }
            }
            return productList;
        }

    }
}
