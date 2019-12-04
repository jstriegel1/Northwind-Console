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
                    Console.WriteLine("Category Database Options:");
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Display one Category and its related products");
                    Console.WriteLine("3) Display all Categories and their related products");
                    Console.WriteLine("4) Add Category");
                    Console.WriteLine("5) Edit a Category");
                    Console.WriteLine("");
                    Console.WriteLine("Product Database Options:");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("6) Display Products");
                    Console.WriteLine("7) Display Specific Product Info");
                    Console.WriteLine("8) Add Product");
                    Console.WriteLine("9) Edit Product Info");
                    Console.WriteLine("");
                    Console.WriteLine("\"q\" to quit");
                    Console.Write("==>");
                    choice = Console.ReadLine();
                    Console.WriteLine("");
                    Console.Clear();
                    logger.Info($"Option {choice} selected");

                    if (choice == "1")
                    {
                        //display all categories
                        var db = new NorthwindContext();
                        DisplayCategories(db);
                    }
                    else if (choice == "2")
                    {
                        //display one category and all of its active related products
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        DisplayCategories(db);
                        Console.Write("==>");
                        int id = int.Parse(Console.ReadLine());
                        Console.WriteLine("");
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Console.WriteLine("");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products.Where(ap => ap.Discontinued == false))
                        {
                            Console.WriteLine(p.ProductName);
                        }
                        Console.WriteLine("");
                    }
                    else if (choice == "3")
                    {
                        //display all categories and all of their active related products
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products.Where(ap => ap.Discontinued == false))
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                        Console.WriteLine("");
                    }
                    else if (choice == "4")
                    {
                        //add category
                        Category category = new Category();
                        Console.Write("Enter Category Name: ");
                        category.CategoryName = Console.ReadLine();
                        Console.Write("Enter the Category Description: ");
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
                                Console.WriteLine("");
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                //save category to db
                                db.AddCategory(category);
                                logger.Info("Category added - {name}", category.CategoryName);
                                Console.WriteLine("");
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                Console.WriteLine("");
                            }
                        }
                    }
                    else if (choice == "5")
                    {
                        //edit an existing category
                        var db = new NorthwindContext();
                        Console.WriteLine("Choose the category you wish to edit:");
                        var category = GetCategory(db);
                        Category UpdatedCategory = InputCategory(db);
                        if (UpdatedCategory != null)
                        {
                            UpdatedCategory.CategoryId = category.CategoryId;
                            db.EditCategory(UpdatedCategory);
                            logger.Info("Category (id: {categoryid}) updated", category.CategoryId);
                            Console.WriteLine("");
                        }
                    }
                    else if (choice == "6")
                    {
                        //display products depending on user choice
                        string displayChoice;
                        do
                        {
                            Console.WriteLine("Which products would you like displayed?");
                            Console.WriteLine("1) All Products");
                            Console.WriteLine("2) Active Products");
                            Console.WriteLine("3) Discontinued Products");
                            Console.WriteLine("4) Return to main menu");
                            Console.Write("==>");
                            displayChoice = Console.ReadLine();
                            Console.WriteLine("");
                            Console.Clear();
                            logger.Info($"Option {displayChoice} selected");
                            Console.WriteLine("");

                            if (displayChoice == "1")
                            {
                                //display all products
                                var db = new NorthwindContext();
                                var query = db.Products.OrderBy(p => p.ProductName);

                                Console.WriteLine($"{query.Count()} product records returned");
                                Console.WriteLine("");
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.ProductID}) {item.ProductName}");
                                }
                                Console.WriteLine("");
                            }
                            else if (displayChoice == "2")
                            {
                                //display all ative products
                                var db = new NorthwindContext();
                                var query = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductName);

                                Console.WriteLine($"{query.Count()} active product records returned");
                                Console.WriteLine("");
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.ProductID} - {item.ProductName}");
                                }
                                Console.WriteLine("");
                            }
                            else if (displayChoice == "3")
                            {
                                //display all discontinued products
                                var db = new NorthwindContext();
                                var query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductName);

                                Console.WriteLine($"{query.Count()} discontinued product records returned");
                                Console.WriteLine("");
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.ProductID} - {item.ProductName}");
                                }
                                Console.WriteLine("");
                            }

                        }
                        while (displayChoice != "4");

                    }
                    else if (choice == "7")
                    {
                        //display all info for a specific product
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
                        Console.WriteLine("");
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
                        Console.WriteLine("");

                    }
                    else if (choice == "8")
                    {
                        //add new product
                        var db = new NorthwindContext();
                        Product product = new Product();

                        Console.Write("Enter Product Name: ");
                        product.ProductName = Console.ReadLine();

                        Console.WriteLine("Enter the Supplier ID from the list below:");

                        DisplaySuppliers(db);

                        Console.Write("==>");

                        product.SupplierId = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter the Cateogry ID from the list below:");

                        DisplayCategories(db);

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

                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                                Console.WriteLine("Product was not added to the database");
                                Console.WriteLine("");
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                //save category to db
                                db.AddProduct(product);
                                logger.Info("Product added - {name}", product.ProductName);
                                Console.WriteLine("");
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                            Console.WriteLine("");
                        }
                    }
                    else if (choice == "9")
                    {
                        //edit an existing product
                        var db = new NorthwindContext();
                        Console.WriteLine("Choose the product you wish to edit:");
                        var product = GetProduct(db);
                        Product UpdatedProduct = InputProduct(db);
                        if (UpdatedProduct != null)
                        {
                            UpdatedProduct.ProductID = product.ProductID;
                            db.EditProduct(UpdatedProduct);
                            logger.Info("Product (id: {productid}) updated", product.ProductID);
                            Console.WriteLine("");
                        }
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
            //ask user for all necessary information to add/edit a product
            Product product = new Product();
            Console.Write("Enter Product Name: ");
            product.ProductName = Console.ReadLine();
            Console.WriteLine("");

            Console.WriteLine("Enter the Supplier ID from the list below:");

            DisplaySuppliers(db);

            Console.Write("==>");
            product.SupplierId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("");

            Console.WriteLine("Enter the Cateogry ID from the list below:");

            DisplayCategories(db);

            Console.Write("==>");
            product.CategoryId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("");

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

            Console.WriteLine("Discontinued? True/False");
            Console.Write("==> ");
            product.Discontinued = Convert.ToBoolean(Console.ReadLine());

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
                Console.WriteLine("");
            }
            return null;
        }

        public static Category InputCategory(NorthwindContext db)
        {
            //ask user for all necessary information to add/edit a category
            Category category = new Category();
            Console.Write("Enter Category Name: ");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("");
            Console.Write("Enter Category Description: ");
            category.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                return category;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                Console.WriteLine("");
            }
            return null;
        }

        public static Product GetProduct(NorthwindContext db)
        {
            //display all products
            //get and return user's choice
            var query = db.Products.OrderBy(p => p.ProductID);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductID}) {item.ProductName}");
            }
            Console.Write("==>");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("");
            Console.Clear();

            Product product = db.Products.FirstOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                return product;
            }
            else
            {
                logger.Error("Invalid Product Id");
                Console.WriteLine("");
                return null;
            }
        }

        public static Category GetCategory(NorthwindContext db)
        {
            //display all categories
            //get and return user's choice
            var query = db.Categories.OrderBy(c => c.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName} - {item.Description}");
            }
            Console.Write("==>");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("");
            Console.Clear();

            Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                return category;
            }
            else
            {
                logger.Error("Invalid Category Id");
                Console.WriteLine("");
                return null;
            }
        }

        public static void DisplaySuppliers(NorthwindContext db)
        {
            // display all suppliers
            var suppliers = db.Suppliers.OrderBy(s => s.SupplierId);
            foreach (Supplier s in suppliers)
            {
                Console.WriteLine($" {s.SupplierId}) {s.CompanyName}");
            }


        }

        public static void DisplayCategories(NorthwindContext db)
        {
            //display all categories
            var categories = db.Categories.OrderBy(c => c.CategoryId);
            foreach (Category c in categories)
            {
                Console.WriteLine($" {c.CategoryId}) {c.CategoryName} - {c.Description}");
            }
        }
    }
}