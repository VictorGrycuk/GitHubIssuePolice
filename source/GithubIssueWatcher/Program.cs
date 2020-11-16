using CommandLine;
using GithubIssueWatcher.Models;
using System;
using System.IO;

namespace GithubIssueWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Arguments, EncryptionArguments>(args)
            .WithNotParsed(errors => { 
                throw new Exception();
            })
            .MapResult(
              (Arguments options) => Run(options),
              (EncryptionArguments options) => Encryption(options),
              errors => 1);
        }

        private static int Run(Arguments options)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(options.ConfigurationPath))
                {
                    options.ConfigurationPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Configuration.json");
                }
                var watcher = new GithubIssueWatcher(options);

                watcher.Run();
                
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static int Encryption(EncryptionArguments options)
        {
            try
            {
                GithubIssueWatcher.FileEncryption(options.ConfigurationPath, options.Password, options.Action);

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Arguments GetArguments(string[] args)
        {
            Arguments options = new Arguments();
            Parser.Default.ParseArguments<Arguments>(args)
                .WithParsed(o => { options = o; })
                .WithNotParsed(errors =>
                {
                    var sentenceBuilder = CommandLine.Text.SentenceBuilder.Create();
                    foreach (var error in errors)
                        Console.WriteLine(sentenceBuilder.FormatError(error));
                });

            return options;
        }
    }

    
}
