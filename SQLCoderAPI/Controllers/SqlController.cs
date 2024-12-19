using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using SQLCoderAPI.Interfaces;

namespace SQLCoderAPI.Controllers;

public class SqlQueryRequest
{
    public string Query { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class SqlController : ControllerBase
{
    private readonly IDBService _dbService;
    public SqlController(IDBService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost(Name = "GenerateReport")]
    public async Task<List<Dictionary<string, object>>> GenerateReport(Kernel kernel,[FromBody] SqlQueryRequest request)
    {
        string? input = @$"### Task
						Generate a SQL query to answer [QUESTION] {request.Query} [/ QUESTION]

						### Instructions
						- If you cannot answer the question with the available database schema, return 'I do not know'
                        - For Person belonging to Sales, the PersonType value is 'SP'

						### Database Schema
						This query will run on a database whose schema is represented in this string:
						CREATE TABLE [Person].[Person](
							[BusinessEntityID] [int] NOT NULL, -- Primary Key of the Table and the ID for the Person
                            [PersonType] [nchar](2) NOT NULL, -- Denotes the Person Type with 2 characters
							[FirstName] [dbo].[Name] NOT NULL, -- First Name of the Person
							[LastName] [dbo].[Name] NOT NULL, -- Last Name of the Person
						) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
						GO
						
						[SQL]
						";
        var sqlQuery = await kernel.InvokePromptAsync(input);
        var result = await _dbService.GetSQLResultAsync(sqlQuery.ToString().Trim());
        return result;
    }
}
