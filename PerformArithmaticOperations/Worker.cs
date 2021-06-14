using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PerformArithmaticOperations
{
    public class Worker : BackgroundService
    {
        private static string _folderPathToObserve = @"C:\temp\PerformArithmaticOperations\DumpingFolder";
        private static string _folderPathForOutput = @"C:\temp\PerformArithmaticOperations\OutputFolder";
        private static string _weatherAppEndpoint = "http://localhost:46111/WeatherForecast";
        private readonly ILogger<Worker> _logger;
        private HttpClient client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            if (!Directory.Exists(_folderPathToObserve)) Directory.CreateDirectory(_folderPathToObserve);
            if (!Directory.Exists(_folderPathForOutput)) Directory.CreateDirectory(_folderPathForOutput);
            _logger.LogInformation($"The service 'PerformArithmaticOperations' has started observing changes to folder: {_folderPathToObserve}");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation($"The service 'PerformArithmaticOperations' has stopped observing changes to folder: {_folderPathToObserve}");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string outputStub = string.Empty;
                string[] filePaths = Directory.GetFiles(_folderPathToObserve);

                if (filePaths.Length > 0)
                {
                    var fileContent = File.ReadAllText(filePaths[0]);
                    string[] fragments = fileContent.Split(",");
                    if (fragments.Length != 3)
                    {
                        outputStub = "Incorrect numbers of paramerters supplied. Please try again!";
                    }
                    else
                    {
                        try
                        {
                            outputStub = ((Operations)int.Parse(fragments[0])) switch
                            {
                                Operations.Sum => Operations.Sum.ToString() + " Operation Result: " + (int.Parse(fragments[1]) + int.Parse(fragments[2])),
                                Operations.Subtract => Operations.Subtract.ToString() + " Operation Result: " + (int.Parse(fragments[1]) - int.Parse(fragments[2])),
                                Operations.Multiply => Operations.Multiply.ToString() + " Operation Result: " + (int.Parse(fragments[1]) * int.Parse(fragments[2])),
                                Operations.Divide => Operations.Divide.ToString() + " Operation Result: " + (int.Parse(fragments[1]) / int.Parse(fragments[2])),
                                _ => GetWeatherDetails().Result,
                            };
                        }
                        catch (Exception e)
                        {
                            outputStub = $"Failed to perform operation because of one or more error. [ERROR: {e.Message}]\n\n[STACK TRACE]\n";
                            while ((e.InnerException == null) ? false : true)
                            {
                                e = e.InnerException;
                                outputStub += $"\nEXCEPTION: \n{ e.StackTrace}\n";
                            }
                        }
                    }
                    FileInfo info = new FileInfo(filePaths[0]);
                    var outputFilePath = Path.Combine(_folderPathForOutput, info.Name);
                    File.WriteAllText(outputFilePath, outputStub);
                    File.Delete(filePaths[0]);
                    _logger.LogInformation($"Output of the operation can be found at: {outputFilePath}");
                }

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }

        private async Task<string> GetWeatherDetails()
        {
            _logger.LogInformation($"Performing Service2Service call to WeatherAPIEndpoint: {_weatherAppEndpoint}");
            var response = await client.GetAsync(_weatherAppEndpoint);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation($"API call to 'WeatherAPIEndpoint' was successful.");
            if (!response.IsSuccessStatusCode)
                _logger.LogInformation($"API call to 'WeatherAPIEndpoint' was not successful.");

            var result = Operations.ApiCallToGetWeather.ToString() + " Operation Result: " + response.Content.ReadAsStringAsync().Result;

            return result;
        }
    }
}
