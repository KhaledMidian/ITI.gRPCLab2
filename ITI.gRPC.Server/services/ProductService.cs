using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITI.gRPC.Server.Protos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using static ITI.gRPC.Server.Protos.productService;

namespace ITI.gRPC.Server.services
{
    public class ProductService : productServiceBase
    {
        private ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = "ApiKey")]
        public override async Task<confirmMessage> getProductById(productID request, ServerCallContext context)
        {
            if(products.FirstOrDefault(p=>p.Id== request.Id) == null )
            {
                return await Task.FromResult(new confirmMessage
                {
                    Status = false,
                    Massage = "Product does not Exist"
                });
            }

            return await Task.FromResult(new confirmMessage
            {
                Status = true,
                Massage = "Product Exist"
            });

        }

        public override async Task<productServiceMessage> AddProduct(productForAdd request, ServerCallContext context)
        {

            try {
                products.Add(new product { Id = products.Count() + 1, Name = request.Name, Price = request.Price });
                return await Task.FromResult(new productServiceMessage {
                    Status = true,
                    Massage="The Product added successfuly",
                    Product=products.First(p => p.Id == products.Count())
                });
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(new productServiceMessage
                {
                    Status = true,
                    Massage = ex.Message,
                    Product= new product { Id = products.Count() + 1, Name = request.Name, Price = request.Price }
                });
            }
        }

        public override async Task<productServiceMessage> UpdateProduct(product request, ServerCallContext context)
        {
            if (products.FirstOrDefault(p => p.Id == request.Id) != null)
            {
                try
                {
                    products.FirstOrDefault(p => p.Id == request.Id).Name = request.Name;
                    products.FirstOrDefault(p => p.Id == request.Id).Price = request.Price;
                return await Task.FromResult(new productServiceMessage
                {
                    Status = true,
                    Massage = "The Product updated successfuly",
                    Product = products.FirstOrDefault(p => p.Id == request.Id)
                });
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new productServiceMessage
                    {
                        Status = true,
                        Massage = ex.Message,
                        Product = products.FirstOrDefault(p => p.Id == request.Id)
                    });
                }
            }
            return await Task.FromResult(new productServiceMessage
            {
                Status = true,
                Massage = "Product does not Exist",
                Product =  request
            });
        }

        private List<product> products = new List<product>() {
            new product{ Id=1,Name="product1",Price=10,Quantity=1,Expiredates=Timestamp.FromDateTime(DateTime.Now.AddMonths(1).ToUniversalTime()),Category=ProductCategory.Car},
            new product{ Id=2,Name="product2",Price=20,Quantity=5,Expiredates=Timestamp.FromDateTime(DateTime.Now.AddMonths(1).ToUniversalTime()),Category=ProductCategory.Food},
            new product{ Id=3,Name="product3",Price=30,Quantity=7,Expiredates=Timestamp.FromDateTime(DateTime.Now.AddMonths(1).ToUniversalTime()),Category=ProductCategory.Tech},
            new product{ Id=4,Name="product4",Price=40,Quantity=9,Expiredates=Timestamp.FromDateTime(DateTime.Now.AddMonths(1).ToUniversalTime()),Category=ProductCategory.Car}
        };

        public async override Task<NumberOfInsertedProducts> AddBulkProd(IAsyncStreamReader<productForAdd> requestStream, ServerCallContext context)
        {
            int num = 0;
            await foreach (var request in requestStream.ReadAllAsync())
            {
                this.AddProduct(request, context);
                num++;
            }
            return await Task.FromResult(new NumberOfInsertedProducts() { NumOfInsertedRows = num });
        }

        public async override Task GetProductsByCriteria(ProductCriteriaMsg request, IServerStreamWriter<product> responseStream, ServerCallContext context)
        {
            var filteredProducts = products.AsQueryable();
            filteredProducts = filteredProducts.Where(p => p.Category == request.Category);

            if (request.OrderByPrice)
                filteredProducts = filteredProducts.OrderBy(p => p.Price);

            foreach (var product in filteredProducts)
            {
                var product1 = new product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    Expiredates = product.Expiredates,
                    Category = product.Category
                };

                await responseStream.WriteAsync(product);
            }
        }

    }
}
