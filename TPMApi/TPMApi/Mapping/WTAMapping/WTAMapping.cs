﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMApi.Helpers;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMDataLibrary.BusinessLogic;
using TPMHelper.AfostoHelper.ProductModel;
using WooCommerceNET.WooCommerce.v3;
using Options = TPMHelper.AfostoHelper.ProductModel.Options;
using TaxClass = TPMHelper.AfostoHelper.ProductModel.TaxClass;

namespace TPMApi.Mapping.WTAMapping
{
    public class WTAMapping : WTAHelper
    {
        //Load pre-configured data from appsettings.json 
        private readonly IOptions<AuthorizationPoco> _config;
        
        //WooCommerce product Object (for general use)
        private Product _product;
        //WooCommerce WCObject is used in Product. 
        //In this case we use is to retrieve every product variant.
        private WCObject _wcObject;

        public WTAMapping(IOptions<AuthorizationPoco> config)
        {
            _config = config;
        }

        public async Task<List<JObject>> MappingData(
            List<Product> WooProducts,
            WCObject wcObject)
        {
            //The list of products we ultimately return to POST to afosto API
            var productsAsJObject = new List<JObject>();
            _wcObject = wcObject;

            //Loop through loaded woocommerce products.
            for (var i = 0; i < WooProducts.Count; i++)
            {
                //Select current product that we 
                _product = WooProducts[i];
                //_product = WooProducts[0];

                var products = new AfostoProductPoco()
                {
                    Weight = CheckDecimalProperty(_product.weight),
                    Cost = CheckDecimalProperty(_product.price),
                    Is_Backorder_Allowed = _product.backorders_allowed,
                    Is_Tracking_Inventory = false,
                    Descriptors = SetDescriptors(
                        _product, 
                        _config, 
                        await SetAfostoData("/metagroups")),
                    Items = await SetItems(
                        _wcObject, 
                        await SetAfostoData("/warehouses"), 
                        await SetAfostoData("/pricegroups"), 
                        await TaxClass()),
                    Collections = SetCollections(
                        await SetAfostoData("/collections")),
                    Specifications = SetSpecifications()
                };

                var ProductAsjsonString = JsonConvert.SerializeObject(products);
                JObject productAsJsonObject = JObject.Parse(ProductAsjsonString);

                productsAsJObject.Add(productAsJsonObject);
            }

            return productsAsJObject;
        }

        /*--------------- GET Once! ---------------*/

        //Get taxclass from afosto api. (21%)
        internal async Task<JToken> TaxClass()
        {
            var metaData = await PreloadAfostoData("/taxclasses");
            return metaData[2];
        }

        //Preload necessary afosto product data with given path.
        //Reduces code smell and duplicates
        internal async Task<JArray> SetAfostoData(string path)
        {
            var data = await PreloadAfostoData(path);
            return data;
        }

        //Get for ex. metadata or pricegroups.
        //This is used for every product/product option. Loaded once for performance
        private async Task<JArray> PreloadAfostoData(string location)
        {
            var reqAfostoData = await MigrationMiddelware.GetAfostoData(
                AfostoDataProcessor.GetLastAccessToken()[0],
                _config, location);

            JArray jArrayMetaData = JArray.Parse(reqAfostoData);

            return jArrayMetaData;
        }
    }
}
