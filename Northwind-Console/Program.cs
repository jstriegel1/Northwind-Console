using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NLog;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Display Products");
                    Console.WriteLine("6) Add Product");
                    Console.WriteLine("7) Display Specific Product Info");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    }
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                //save category to db
                                db.AddCategory(category);
                                logger.Info("Category added - {name}", category.CategoryName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "5")
                    {
                        string displayChoice;
                        do
                        {
                            Console.WriteLine("Which products would you like displayed?");
                            Console.WriteLine("1) All Products");
                            Console.WriteLine("2) Active Products");
                            Console.WriteLine("3) Discontinued Products");
                            Console.WriteLine("4) Return to main menu");
                            displayChoice = Console.ReadLine();
                            Console.Clear();
                            logger.Info($"Option {displayChoice} selected");

                            if (displayChoice == "1")
                            {
                                var db = new NorthwindContext();
                                var query = db.Products.OrderBy(p => p.ProductName);

                                Console.WriteLine($"{query.Count()} product records returned");
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                                }
                                Console.WriteLine("");
                            }
                            else if (displayChoice == "2")
                            {
                                var db = new NorthwindContext();
                                var query = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductName);

                                Console.WriteLine($"{query.Count()} active product records returned");
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.ProductID} - {item.ProductName}");
                                }
                                Console.WriteLine("");
                            }
                            else if (displayChoice == "3")
                            {
                                var db = new NorthwindContext();
                                var query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductName);

                                Console.WriteLine($"{query.Count()} discontinued product records returned");
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.ProductID} - {item.ProductName}");
                                }
                                Console.WriteLine("");
                            }

                        }
                        while (displayChoice != "4");
                        
                    }
                    else if (choice == "6")
                    {
                        var db = new NorthwindContext();
                        Product product = new Product();

                        Console.Write("Enter Product Name: ");
                        product.ProductName = Console.ReadLine();

                        Console.WriteLine("Enter the Supplier ID from the list below:");
                        var suppliers = db.Suppliers.OrderBy(s => s.SupplierId);
                        foreach (Supplier s in suppliers)
                        {
                            Console.WriteLine($" {s.SupplierId}) {s.CompanyName}");
                        }
                        Console.Write("==>");
                        product.SupplierId = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter the Cateogry ID from the list below:");
                        var categories = db.Categories.OrderBy(c => c.CategoryId);
                        foreach (Category c in categories)
                        {
                            Console.WriteLine($" {c.CategoryId}) {c.CategoryName}");
                        }
                        Console.Write("==>");
                        product.CategoryId = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Enter Quantity Per Unit: ");
                        product.QuantityPerUnit = Console.ReadLine();

                        Console.Write("Enter Unit Price: ");
                        product.UnitPrice = Convert.ToDecimal(Console.ReadLine());

                        Console.Write("Enter Units in Stock: ");
                        product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
                        //int.TryParse(Console.ReadLine(), out int UnitsInStock);

                        Console.Write("Enter Units on Order: ");
                        product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());

                        Console.Write("Enter Reorder Level: ");
                        product.ReorderLevel = Convert.ToInt16(Console.ReadLine());

                        product.Discontinued = false;

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {

                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                                Console.WriteLine("Product was not added to the database");
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                //save category to db
                                db.AddProduct(product);
                                logger.Info("Product added - {name}", product.ProductName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "7")
                    {
                        var db = new NorthwindContext();
                        var query = db.Products.OrderBy(p => p.ProductID);

                        Console.WriteLine("Select the product for which you want to see more info:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                        }
                        Console.Write("==>");
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"ProductId {id} selected");
                        Product product = db.Products.FirstOrDefault(p => p.ProductID == id);
                        Console.WriteLine($"Product Name: {product.ProductName}");
                        Console.WriteLine($"Supplier ID: {product.SupplierId}");
                        Console.WriteLine($"Category ID: {product.CategoryId}");
                        Console.WriteLine($"Quantity Per Unit: {product.QuantityPerUnit}");
                        Console.WriteLine($"Unit Price: {product.UnitPrice}");
                        Console.WriteLine($"Units in Stock: {product.UnitsInStock}");
                        Console.WriteLine($"Units on Order: {product.UnitsOnOrder}");
                        Console.WriteLine($"Reorder Level: {product.ReorderLevel}");
                        Console.WriteLine($"Discontinued: {product.Discontinued}");

                    }
                    
                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static Product InputProduct(NorthwindContext db)
        {
            Product product = new Product();
            Console.Write("Enter Product Name: ");
            product.ProductName = Console.ReadLine();

            Console.WriteLine("Enter the Supplier ID from the list below:");
            var suppliers = db.Suppliers.OrderBy(s => s.SupplierId);
            foreach (Supplier s in suppliers)
            {
                Console.WriteLine($" {s.SupplierId}) {s.CompanyName}");
            }
            Console.Write("==>");
            product.SupplierId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the Cateogry ID from the list below:");
            var categories = db.Categories.OrderBy(c => c.CategoryId);
            foreach (Category c in categories)
            {
                Console.WriteLine($" {c.CategoryId}) {c.CategoryName}");
            }
            Console.Write("==>");
            product.CategoryId = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter Quantity Per Unit: ");
            product.QuantityPerUnit = Console.ReadLine();

            Console.Write("Enter Unit Price: ");
            product.UnitPrice = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Enter Units in Stock: ");
            product.UnitsInStock = Convert.ToInt16(Console.ReadLine());

            Console.Write("Enter Units on Order: ");
            product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());

            Console.Write("Enter Reorder Level: ");
            product.ReorderLevel = Convert.ToInt16(Console.ReadLine());

            product.Discontinued = false;

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                return product;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }

        public static Product GetProduct(NorthwindContext db)
        {

            var query = db.Products.OrderBy(p => p.ProductID);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductID}) {item.ProductName}");
            }
            Console.Write("==>");
            int id = int.Parse(Console.ReadLine());
            Console.Clear();

            Product product = db.Products.FirstOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                return product;
            }
            else
            {
                logger.Error("Invalid Post Id");
                return null;
            }
        }
    }
}