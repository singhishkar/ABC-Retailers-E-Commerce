<div align="center">

<h1>🚀💻 ABC Retailers E-Commerce Store Website 🏬🏷️</h1>

</div>

---

## 📑 Table of Contents

✨ 1. [**Introduction**](#-1-introduction)<br>
💻 2. [**Setting Up the Project Locally**](#-2-setting-up-the-project-locally)<br>
✅ 3. [**Features and Functionality**](#-3-features-and-functionality)<br>
🖼️ 4. [**Screenshots**](#️-4-screenshots)<br>
🔑 5. [**Admin Login Credentials**](#-5-admin-login-credentials)<br>
🏗️ 6. [**Architecture**](#️-6-architecture)<br>
👥 7. [**Author and Contributions**](#-7-author-and-contributions)<br>
⚖️ 8. [**MIT License**](#️-8-mit-license)<br>
❓ 9. [**Frequently Asked Questions (FAQ)**](#-9-frequently-asked-questions-faq)<br>
📚 10. [**References**](#-10-references)<br>

---

## ✨ 1. Introduction

Welcome to the **ABC Retailers** project, a comprehensive solution designed to modernize the aging order processing and data management infrastructure of **ABC Retail**, a rapidly growing online retailer. ABC Retail was struggling with slow transaction processing, storage inefficiencies for product images, and message delivery delays, especially during peak shopping seasons. This project delivers a possible scalable, reliable, and cost-effective cloud-based architecture solution utilizing **Microsoft Azure Services**. By leveraging **Azure's** robust capabilities, the new system addresses operational bottlenecks, improves data analytics, and lays the groundwork for personalizing customer experiences and optimizing product recommendations. Explore the code and deployment links to see how this solution transforms a legacy system into a high-performance cloud application ready for demanding e-commerce operations.

### Technical Hightlights:

- **Data Storage**: Customer profiles, product information, and order details are managed using a combination of **Azure Tables** and **Azure SQL Database**.
- **Media Hosting**: Product images and multimedia content are hosted efficiently using **Azure Blob Storage**.
- **Transactional Messaging**: Details for order processing and inventory management are reliably stored and processed via **Azure Queues**.
- **Document Management**: Contracts and log files are securely stored using **Azure Files**.
- **Serverless Computing**: The application logic is enhanced with **Azure Functions** to improve scalability and cost-effectiveness for storage operations.
- **Disaster Recovery/High Availability**: An **Azure SQL Database** is implemented in a different region to enhance reliability and data continuity.
- **Deployment**: The entire web application was previously deployed to an **Azure App Service** for online accessibility.

### Overview of POE Part Breakdown

The project was developed in **three progressive parts**, with each part building upon the foundational work of the previous one:

- **Part 1 (Previous Iterations): Azure Storage Solution (Formative)**: This phase focused on developing the initial web application and integrating various **Azure Storage Services**. The key deliverables included implementing **Azure Tables, Azure Blob Storage, Azure Queues, and Azure Files** to handle different data types for ABC Retail's operations.
- **Part 2 (Previous Iterations): Integrating more Azure Services (Formative)**: This introduced **Azure Functions** to interact with the storage services, thereby enhancing scalability and cloud suitability.
- **Part 3 (ABC Retail): Analysing the Cloud Solution (Summative)**: The final, summative part refined the solution by migrating core structured data to an **Azure SQL Database** and creating a regional replica for resilience. Furthermore, a complete UI overhaul/update was executed to modernize the user experience and interface design.

---

## 💻 2. Setting Up the Project Locally

### Prerequisites

To successfully compile and run this project, you must have the following installed on your system:

- **Operating Systems**: Any OS compatible with the .NET 8.0 Runtime and the corresponding SDK. This generally includes modern versions of Windows (Windows 10/11), macOS 10.15+, or Linux distributions that support the .NET 8 framework.
- **IDE**: Compatible version of Microsoft Visual Studio 2019+ (or an equivalent IDE like VS Code with extensions, such as C# Dev Kit and Azure Functions.).
- **Version Control**: Git for cloning the repository.
- **Database**: SQL Server instance (either local or remote) is necessary to integrate with the main data store.
- **Frameworks**:
  - Target Framework: .NET 8.0 (net8.0)
  - Web Framework: ASP.NET Core 8.0
  - Serverless Framework: Azure Functions v4
- **RAM**: Minimum 4GB
- **Disk Space**: Minimum 100MB free space
- **Dependencies**:
  - ABCRetail.csproj:
    - Azure.Data.Tables (12.9.0)
    - Azure.Storage.Blobs (12.21.2)
    - Azure.Storage.Common (12.20.1)
    - Azure.Storage.Files.Shares (12.19.1)
    - Azure.Storage.Queues (12.19.1)
    - BouncyCastle.Cryptography (2.4.0)
  - FunctionApp1.csproj:
    - Azure.Data.Tables (12.9.1)
    - Azure.Storage.Blobs (12.21.2)
    - Azure.Storage.Files.Shares (12.19.1)
    - Azure.Storage.Queues (12.19.1)
    - Microsoft.ApplicationInsights.WorkerService (2.22.0)
    - Microsoft.Azure.Functions.Worker (1.23.0)
    - Microsoft.Azure.Functions.Worker.ApplicationInsights (1.2.0)
    - Microsoft.Azure.Functions.Worker.Extensions.Http (3.1.0)
    - Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore (1.2.0)
    - Microsoft.Azure.Functions.Worker.Sdk (1.17.4)
    - Microsoft.Azure.WebJobs (3.0.41)
    - Newtonsoft.Json (13.0.3)

### Project Configurations

#### `ABCRetail.csproj`

This configuration defines the project as an **ASP.NET Core web application** targeting the **latest framework version**.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Azure.Data.Tables" Version="12.9.0" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.21.2" />
    <PackageReference Include="Azure.Storage.Common" Version="12.20.1" />
    <PackageReference Include="Azure.Storage.Files.Shares" Version="12.19.1" />
    <PackageReference Include="Azure.Storage.Queues" Version="12.19.1" />
    <PackageReference Include="BouncyCastle.Cryptography" Version="2.4.0" />
  </ItemGroup>

</Project>
```

#### `appsettings.json`

This configuration stores **connection strings**, **custom settings**, and **logging configuration**, which are loaded at runtime.

```json
{
  "ConnectionStrings": {
    "ABCRetail": "Server=YOUR_SERVER;Database=ABCRetailDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "AzureStorage": "DefaultEndpointsProtocol=https;AccountName=YOUR_ACCOUNT;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net"
  },
  "FunctionUrl": "https://YOUR_FUNCTION_APP.azurewebsites.net/api/",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### `FunctionApp1.csproj`

This configuration defines the project as an **Azure Function** targeting the **latest framework version**.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <AzureFunctionsVersion>v4</AzureFunctionsVersion>
    <OutputType>Exe</OutputType>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Azure.Data.Tables" Version="12.9.1" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.21.2" />
    <PackageReference Include="Azure.Storage.Files.Shares" Version="12.19.1" />
    <PackageReference Include="Azure.Storage.Queues" Version="12.19.1" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker" Version="1.23.0" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http" Version="3.1.0" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore" Version="1.2.0" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.17.4" />
    <PackageReference Include="Microsoft.ApplicationInsights.WorkerService" Version="2.22.0" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.ApplicationInsights" Version="1.2.0" />
    <PackageReference Include="Microsoft.Azure.WebJobs" Version="3.0.41" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\ABCRetail\ABCRetail.csproj" />
  </ItemGroup>
  <ItemGroup>
    <None Update="host.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="local.settings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <CopyToPublishDirectory>Never</CopyToPublishDirectory>
    </None>
  </ItemGroup>
  <ItemGroup>
    <Using Include="System.Threading.ExecutionContext" Alias="ExecutionContext" />
  </ItemGroup>
</Project>

```

### Installation

Follow these steps to get the application running on your local machine.

#### 1. Clone the Repository

- Naviagte and click the green "**Code**" button at the top of this repository.
- Copy the URL under the "**HTTPS**" tab (`https://github.com/singhishkar/ABC-Retailers-E-Commerce.git`).
- Navigate to the directory where you want to save the project (e.g., `cd Documents/Projects`).
- Open your preferred terminal or command prompt and use the following command to clone the project:

```bash
git clone https://github.com/singhishkar/ABC-Retailers-E-Commerce.git
```

- This will create a new folder with the repository's name and download all the files and the entire history into it.
- Alternatively, you may download as a **ZIP file** or clone using **GitHub Desktop**.

#### 2. Open in Visual Studio (Recommended)

1.  Open **Visual Studio 2022**.
2.  Navigate to **File \> Open \> Project/Solution**.
3.  Browse to the cloned repository and select the **Solution file (.sln)** to load the project.
4.  Visual Studio will automatically perform a package restore (`dotnet restore`).

The application will launch. You should see a message in the console indicating the application is running. The browser should open automatically to the default URL.

#### 3. Configure Database Connection

The application connects to a **SQL database**. You must configure the connection string in the `appsettings.json` file. Create this file if it doesn't exist, and add the configuration using a placeholder structure.

> **Note**: If you are running locally, you will typically use a connection string pointing to a local SQL Server instance (e.g., using LocalDB).

#### 4. Apply Database Migrations

Use the **Entity Framework Core tools** to create the database schema based on the code's models. Run these commands from the main project directory (`ABCRetail`):

```bash
# Update the database to the latest migration
dotnet ef database update
```

This command will create the `ABCRetailDB` database (if it doesn't exist) and apply all necessary tables.

> #### ⚠️ Note: Using Azure Features
>
> 1. **Publish the Azure Function to your Azure Subscription**
>
> - Deploy your Azure Function to the appropriate resource group and Function App within your Azure subscription using Visual Studio, Azure CLI, or the Azure Portal.
>
> 2. **Retrieve and Configure the Function URL**
>
> - After publishing, navigate to your Function App in the Azure Portal, open the specific function, and copy the generated Function URL.
> - Insert this value into the FunctionUrl field within your appsettings.json file.
>
> 3. **Provide the Azure Storage Connection String**
>
> - Replace the placeholder value for AzureStorage in appsettings.json with your actual Azure Storage connection string.
> - This connection string must include both the AccountName and the corresponding AccountKey.
>
> 4. **Protect Sensitive Credentials**
>
> - Avoid committing sensitive information such as access keys or connection strings to GitHub.
> - For local development, store these values in a .env file or user secrets.
> - For production deployments, use Azure App Service Configuration Settings, Azure Key Vault, or other secure credential management solutions.

### Running

#### 1. Run in Visual Studio

1.  Select **Build \> Build Solution** (or press `F6`) to compile the project.
2.  Click the **Run** button (or press `F5`) to start the application with debugging, or `Ctrl+F5` to start without debugging.

#### 2. Run via Command Line (Alternative)

If you are using **Visual Studio Code** or prefer the CLI:

1.  Navigate to the project directory containing the `.csproj` file.
2.  Execute the following commands in sequence:

```bash
# Clean up any previous build files
dotnet clean

# Restore project dependencies
dotnet restore

# Build and run the application
dotnet run
```

#### 4. Access the Application

- The console output will indicate the local URL where the application is hosted (e.g., `https://localhost:7198`).
- Open your web browser and navigate to the displayed URL (e.g., `https://localhost:7198`). You should now see the **ABC Retailers home page**.

---

## ✅ 3. Features and Functionality

### Azure Storage and Data Functionalities (POE Part 1)

This part focuses on using various **Azure Storage types** to address different data needs:

- **Customer/Product Data (Azure Tables)**:
  - Store customer profiles and product-related information using Azure Tables.
- **Media Content (Azure Blob Storage)**:
  - Host images and other multimedia content (e.g., product images) using Azure Blob Storage.
- **Order/Inventory Messaging (Azure Queues)**:
  - Store details relating to the processing of orders and inventory management in Azure Queues.
  - Examples of stored messages include "Uploading image 'imageName'", "Processing order", etc.
- **Document and Log Storage (Azure Files)**:
  - Store contracts and log files using Azure Files.

### Serverless Integration and Improvement (POE Part 2)

This phase integrates **Azure Functions** to enhance architecture flexibility and performance:

- **Azure Functions Integration**: **Function1** is integrated into the code to store, write, read and send a file to Azure Tables, Azure Blob Storage, Azure Queue, and Azure Files respectively. This enhances scalability, cost-effectiveness, and cloud suitability.

### Data Centralization and Resilience (POE Part 3)

The final phase introduced a relational database for centralized data storage and ensures application resilience:

- **Centralized Data Storage (Azure SQL Database)**:
  - Creation and implementation of an Azure SQL database to ensure centralized data storage and enable data analysis.
  - The database stores customer information, product information, and order information.
- **High Availability/Disaster Recovery (Database Replica)**:
  - Creation of a replica of the Azure SQL database in a different region.
  - Provision of a written motivation explaining the necessity of a replica database.

---

## 🖼️ 4. Screenshots

The below are screenshots of the website for Part 3, hosted in Australia East region.

|            Heading            |                        Image / Screenshots                         |
| :---------------------------: | :----------------------------------------------------------------: |
|     **Welcome/Home Page**     |           ![Welcome/Home Page](Screenshots/HomePage.png)           |
|       **Checkout Page**       |             ![Checkout Page](Screenshots/Checkout.png)             |
|    **My Cart/Users Cart**     |         ![My Cart/Users Cart Page](Screenshots/MyCart.png)         |
|        **Craftworks**         |              ![Craftworks](Screenshots/Craftwork.png)              |
|   **Add Prodcuts (Admin)**    |        ![Add Prodcuts (Admin)](Screenshots/AddProduct.png)         |
| **Registration Confirmation** | ![Registration Confirmation](Screenshots/RegisterConfirmation.gif) |

---

## 🔑 5. Admin Login Credentials

**Email**: _admin@gmail.com_ <br>
**Password**: _Admin@123_ <br>

> ⚠️ **Note**: For demonstration purposes, the Admin credentials are given to sign in directly.
> This approach is **not recommended** for production environments. In a real deployment, several administrator accounts would be pre-seeded and roles securely assigned through controlled provisioning processes.

---

## 🏗️ 6. Architecture

he solution for **ABC Retail** is built in mind to be used as a **cloud-native application** utilizing **Microsoft Azure services** for maximum scalability, reliability, and cost-efficiency.

### Core Components and Application Host

- **Web Application (ASP.NET MVC/Core)**:
  - Serves as the central user interface and application logic, allowing users to interact with all backend services.
  - Designed to adhere to modern design patterns for separation of concerns and maintainability.
- **Deployment Host (Azure App Service)**:
  - The entire web application is built to be deployed here, providing a fully managed **Platform as a Service (PaaS) environment**.
  - Ensures the application is highly available and accessible via a **unique public URL**.

### Application Structure (ASP.NET Core MVC)

The application code adheres to the **MVC pattern**, which ensures a clear separation of concerns, making the codebase maintainable, testable, and scalable.

- **Model**: This layer manages the application's data and business logic. It includes the Entity Framework Core data context, the entity classes (e.g., Product, Order), and the service classes responsible for interacting with the database and external Azure APIs.
- **View**: The user interface (UI) is rendered using Razor views. This layer is responsible solely for presenting the data to the client and capturing user input.
- **Controller**: Controllers act as the entry point for handling user requests. They receive input, coordinate the necessary actions by calling methods in the Model layer, and determine which View to return to the user.

### Backend Services and Data Storage

The architecture intentionally leverages a **polyglot persistence strategy**, using the best-fit Azure service for each type of data and operational requirement:

- **Centralized Structured Data (Azure SQL Database)**:
  - Stores core relational data, including definitive customer, product, and order information, enabling complex queries and data analysis.
  - The use of SQL ensures data integrity, transactional consistency, and support for the relational data model.
- **Semi-Structured/NoSQL Data (Azure Table Storage)**:
  - Used for storing high-volume, schemaless data such as customer profiles and less frequently accessed product details.
  - Provides a cost-effective and highly scalable NoSQL key/attribute store.
- **Media and Object Storage (Azure Blob Storage)**:
  - Hosts unstructured data, specifically product images and other multimedia content.
  - Optimized for massive scalability and cost-effective storage of binary files.
- **Asynchronous Messaging (Azure Queues)**:
  - Acts as a temporary store for messages related to order processing and inventory updates (e.g., "Processing Order ID X").
  - Decouples the web application from backend processing tasks, improving performance and fault tolerance.
- **Shared File Storage (Azure Files)**:
  - Used for securely storing corporate documents, such as contracts, and application-generated log files, which require a standard file share interface.

---

## 👥 7. Author and Contributions

### Primary Developer:

- I, **_Ishkar Singh_**, am the sole developer and author of this project:
  Email (for feedback or concerns): **ishkar.singh.108@gmail.com**

### Reporting Issues:

- If you encounter any bugs, glitches, or unexpected behaviour, please open an Issue on the GitHub repository.
- Provide as much detail as possible, including:
  - Steps to reproduce the issue
  - Error messages (if any)
  - Screenshots or logs (if applicable)
  - Expected vs. actual behaviour
- Clear and descriptive reports help improve the project effectively.

### Proposing Enhancements:

- Suggestions for improvements or feature enhancements are encouraged.
- You may open a Discussion or submit an Issue describing the proposed change.
- All ideas will be reviewed and considered for future updates.

---

## ⚖️ 8. MIT License

**Copyright © 2025 Ishkar Singh**<br>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

## ❓ 9. Frequently Asked Questions (FAQ)

### 1. Why was Azure SQL Database chosen over Azure Table Storage for core order and customer data?

**Azure SQL Database** was selected for core data (orders, primary customer profiles) because it provides a relational model with transactional integrity (ACID properties), complex querying capabilities (joins), and defined schemas essential for financial and operational reporting. **Azure Table Storage** is reserved for high-volume, non-relational, schemaless data where cost and simple key-based lookups are the main priorities.

### 2. What is the primary purpose of using Azure Functions in this architecture?

The main goal is to introduce **serverless computing** and **decouple operational tasks**. The functions handle specific, discrete tasks—like processing a message from a queue or writing a record to a storage table—without requiring a continuously running server, which significantly enhances scalability and reduces operational costs.

### 3. How is the system protected against data loss?

Data loss protection and high availability are primarily made to be achieved through the implementation of an **Azure SQL Database Replica** established in a **different geographical region**. This ensures core operational data can be recovered quickly and maintains business continuity during a catastrophic regional failure.

### 5. Where are product images stored, and how are they accessed?

Product images are stored in **Azure Blob Storage**. This service is optimized for hosting massive amounts of unstructured data. Images are accessed via unique, high-performance URLs generated by the Blob Storage service, which the web application uses for display.

### 6. What is the role of Azure Queues in order processing?

**Azure Queues** manage the **reliable flow of transactional messages** related to orders (e.g., inventory deduction, fulfillment notifications). They act as a buffer, ensuring that messages are reliably stored and delivered to the backend processing Azure Functions, preventing backlogs and system slowdowns during high-traffic events.

### 7. Why are both Azure SQL Database and Azure Table Storage used for customer/product data?

They serve different purposes:
_ **Azure SQL** holds the **relational data** (e.g., customer transaction history, product inventory levels).
_ **Azure Table Storage** is used for **supplementary, semi-structured data** like user preferences, session history, or application metadata, where massive volume and speed of simple writes are crucial.

### 8. Is it possible to execute the application locally without an active connection to the Azure Cloud services?

The **foundational web application component** can be executed locally, for instance, using a local development environment. However, any **core feature reliant on the cloud resources** - including data retrieval from Azure SQL Database, media access from Blob Storage, and the initiation of asynchronous message processing via Azure Queues/Functions will be **non-operational or disabled**. Full functionality requires connectivity to the live, provisioned Azure backend.

### 9. Is the processing and confirmation of customer orders performed in a synchronous or asynchronous manner?

Order processing is **designed to be asynchronous** to maximize application responsiveness and scalability. Upon order submission, the transaction details are **immediately placed into an Azure Queue**. This action allows the front-end web application to confirm receipt instantly.

### 10. Are the provided default user credentials suitable for use in a production environment?

**Absolutely not**. The **authentication credentials** included within the repository or initial setup files are **strictly intended for development, testing, and demonstration purposes only**. Production environments must utilize a robust set of accounts, ideally provisioned through secure management practices and possibly integrated with services like Azure Active Directory, ensuring no default or easily compromised credentials are used for live operations.

### 11. How are authentication mechanisms and data security implemented within the web application?

Security is handled through multiple integrated layers:
_ **Authentication**: The system employs role-based authentication, restricting access and functionality based on a user's assigned permissions (e.g., Administrator, Customer).
_ **Data Security**: All data transmission across the network is secured via HTTPS/SSL when published on Azure, and sensitive data stored in the Azure SQL Database benefits from Azure's platform-level encryption and management features.

---

## 📚 10. References

- **Azure, n.d. Azure SDK for .NET.** [online] _[github.com](https://github.com/Azure/azure-sdk-for-net/)_ [Accessed 12 April 2024].
- **BroCode, n.d. C# for Beginners.** [online] _[youtube.com](https://www.youtube.com/watch?v=4BwyqmRTrx8)_ [Accessed 3 May 2024].
- **BroCode, n.d. C# Full Course.** [online] _[youtube.com](https://www.youtube.com/watch?v=T_zSVIsTEZU)_ [Accessed 15 April 2024].
- **CodePen, n.d. Nikki Peel – RwavQer.** [online] _[codepen.io](https://codepen.io/nikki-peel/pen/RwavQer)_ [Accessed 28 May 2024].
- **Microsoft Docs, n.d. Add User Data – ASP.NET Core Authentication.** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/add-user-data?view=aspnetcore-5.0&tabs=visual-studio)_ [Accessed 11 June 2024].
- **Microsoft Docs, n.d. ASP.NET Core Fundamentals – Static Files.** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-8.0)_ [Accessed 2 April 2024].
- **Microsoft Docs, n.d. Azure Blob Storage Overview.** [online] _[learn.microsoft.com](https://learn.microsoft.com/azure/storage/blobs/)_ [Accessed 7 May 2024].
- **Microsoft Docs, n.d. Azure Functions Overview.** [online] _[learn.microsoft.com](https://learn.microsoft.com/azure/azure-functions/)_ [Accessed 10 May 2024].
- **Microsoft Docs, n.d. Create Your First Azure Function (Visual Studio).** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio)_ [Accessed 20 June 2024].
- **Microsoft Docs, n.d. First MVC App Tutorial.** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0&tabs=visual-studio)_ [Accessed 9 May 2024].
- **Microsoft Docs, n.d. Razor Pages Tutorial.** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/razor-pages-start?view=aspnetcore-8.0&tabs=visual-studio)_ [Accessed 16 April 2024].
- **Microsoft Docs, n.d. Roles in ASP.NET Core Authorization.** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-8.0)_ [Accessed 29 April 2024].
- **Microsoft Docs, n.d. What is Azure Cognitive Search?** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/azure/search/search-what-is-azure-search)_ [Accessed 8 June 2024].
- **YouTube, n.d. Authentication Tutorial.** [online] _[youtube.com](https://www.youtube.com/watch?v=qvsWwwq2ynE)_ [Accessed 14 May 2024].
- **YouTube, n.d. Authorization Roles Tutorial.** [online] _[youtube.com](https://www.youtube.com/watch?v=ghzvSROMo_M)_ [Accessed 1 June 2024].
- **YouTube, n.d. Azure Search Overview.** [online] _[youtube.com](https://www.youtube.com/watch?v=g15mF_XAOB8&t=309s)_ [Accessed 24 June 2024].
