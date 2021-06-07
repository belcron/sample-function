using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp2
{
    public interface IRecipeClient
    {
        Task<string> DownloadRecipe();

        Task SaveRecipe(string recipe);

        string ContainerName { get; set; }
    }
}
