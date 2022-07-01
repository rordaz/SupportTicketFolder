using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fclp;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;

namespace vsmTicketFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a generic parser for the ApplicationArguments type
            var parser = new FluentCommandLineParser<ApplicationArguments>();

            parser.Setup(arg => arg.Customer)
            .As('c', "customer")
            //.SetDefault(@"Acme")
            .WithDescription("Customer Name");
            //.Required();

            parser.Setup(arg => arg.Product)
            .As('p', "product")
            //.SetDefault(@"ProductAlpha")
            .WithDescription("Product on Support");
            //.Required();

            parser.Setup(arg => arg.TicketId)
            .As('t', "ticketId")
            //.SetDefault(@"20220731010")
            .WithDescription("VSM Ticket ID");
            //.Required();

            parser.Setup(arg => arg.Description)
             .As('d', "Description")
             .SetDefault(@"Missing Records")
             .WithDescription("Ticket Description");
            //.Required();

            parser.Setup(arg => arg.InstallEnvVar)
             .As('e', "enviromentVars")
             .SetDefault(false)
             .WithDescription("Set Enviroment Variables");
            //.Required();

            string  helpText = @"vsmTicketFolder -c Acme -p ProductAlpha -t 20220731002 -d ""Brief Description Issue""";

            parser.SetupHelp("?", "help")
            //.Callback(text => Console.WriteLine("001: Use Example: " + helpText));
            .Callback(text => Console.WriteLine(text));


            var result = parser.Parse(args);

            bool enviromentVars = parser.Object.InstallEnvVar;

            if (enviromentVars)
            {
                Utils.setEnviromentVars();
                return;
            }

            string ticketId = parser.Object.TicketId;
            string customerName = parser.Object.Customer;
            string productName = string.Empty;
            if (parser.Object.Product != null)
                productName = parser.Object.Product;
            else
                productName = ConfigurationManager.AppSettings.Get("defaultProduct").ToString();

            //Utils.setEnviromentVars();

            bool missingParameters = false;

            parser.HelpOption.ShowHelp(parser.Options);

            if (string.IsNullOrEmpty(ticketId) || string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(productName) & !result.HelpCalled)
            {
                Console.WriteLine("");
                Console.WriteLine("Missing parameters ticketId, customerName or productName");
                //parser.HelpOption.ShowHelp(parser.Options);
                missingParameters = true;
            }

            string description = parser.Object.Description;
            // Single Line
            string NotesString = "["+ customerName + "] "+"VSM " + ticketId + ": " + description;
            // Multiple Lines -- not in use
            string[] NotesStringlines = { "First line", "Second line", "Third line" };

            string baseTicketFolder = ConfigurationManager.AppSettings.Get("dirPath").ToString();

            // Read from the config Filecls
            NameValueCollection CommonFolders = System.Configuration.ConfigurationManager.GetSection("CommonFolders") as NameValueCollection;
            NameValueCollection ProductAlphaFolders = System.Configuration.ConfigurationManager.GetSection("ProductAlphaFolders") as NameValueCollection;
            NameValueCollection ProductBetaFolders = System.Configuration.ConfigurationManager.GetSection("ProductBetaFolders") as NameValueCollection;

            if (result.HasErrors == false && !result.HelpCalled && !missingParameters)
            {
                try
                {
                    string newDirectory = baseTicketFolder + "\\" + customerName + "\\" + productName + "\\" + ticketId + " - " + description;
                    Directory.CreateDirectory(newDirectory);

                    // Create the notes
                    System.IO.File.WriteAllText(newDirectory+@"\Notes.txt", NotesString);

                    foreach (var key in CommonFolders.AllKeys)
                    {
                        Directory.CreateDirectory(newDirectory + "\\" + CommonFolders[key]);
                    }

                    if (productName.Equals("ProductAlpha"))
                    {
                        foreach (var key in ProductAlphaFolders.AllKeys)
                        {
                            Directory.CreateDirectory(newDirectory + "\\" + ProductAlphaFolders[key]);
                        }
                    }

                    if (productName.Equals("ProductBeta"))
                    {
                        foreach (var key in ProductBetaFolders.AllKeys)
                        {
                            Directory.CreateDirectory(newDirectory + "\\" + ProductBetaFolders[key]);
                        }
                    }

                    if (Directory.Exists(newDirectory))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        { Arguments = newDirectory, FileName = "explorer.exe" };
                        Process.Start(startInfo);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

    }
    public class ApplicationArguments
    {
        public string Customer { get; set; }
        public string Product { get; set; }
        public string TicketId { get; set; }
        public string Description { get; set; }
        public bool InstallEnvVar { get; set; }

    }
}
  
