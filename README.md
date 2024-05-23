# Burger King Website 

- A web application for selling hamburgers.
- Check out the live version of the website: [BurgerKingWebsite](http://www.burgerking.kcntt.edu.vn/)

## Previews Website
- For Customer
  
![Customer_Interface](/BurgerKing/images/user_interface.png)

- For Admin

![Admin_Interface](/BurgerKing/images/admin_interface.png)

## Table of Contents

- [About the Project](#about-the-project)
- [Installation](#installation)
- [Usage](#usage)
- [Contact](#contact)

## About the Project

Burger King Website is a web application built using ASP.NET MVC that allows customers to browse a selection of hamburgers, place orders, and make payments through MoMo. Administrators have the ability to add, delete, and edit product listings to manage the hamburger offerings.

### Built With

- [ASP.NET MVC](https://dotnet.microsoft.com/apps/aspnet/mvc)
- [Entity Framework](https://docs.microsoft.com/en-us/ef/)
- [Bootstrap](https://getbootstrap.com/)
- [MoMo Payment Gateway](https://momo.vn/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Installation

1. Clone the repository: git clone https://github.com/NgoCongHuan/BurgerKing-Website
2. Open SQL Server Management Studio and run script in BurgerKingDatabase.sql
3. Open the solution in Visual Studio
4. Change 'Connection String' in web.config
5. Rebuild solution
6. Run the project

Accounts in the database: 
| Username                | Password      | Role           |
|-------------------------|---------------|----------------|
| customer1@gmail.com     | customer1     | user           |
| admin1@gmail.com        | admin1        | administrator  |

## Usage
### Customer Interface
- Add new products to cart
- Edit products in the shopping cart
- Proceed to order and pay with Momo wallet
- View order history
- Register/Log in
- Edit account information
### Admin Interface
- CRUD for all objects including: categories, products, accounts, orders

## Contact
If you have any questions, please contact via email at ngohuan18112002@gmail.com
