using CommandLine;

namespace GithubIssueWatcher.Models
{
    [Verb("run", HelpText = "Runs the application.")]
    public class Arguments
    {
        [Option('c', "configuration", Required = false, HelpText = "File path to the configuration file. Defaults to execution folder.")]
        public string ConfigurationPath { get; set; }

        [Option('p', "password", Required = false, HelpText = "Password to decrypt the configuration file. If not provided, the app will assume the file is decrypted.")]
        public virtual string Password { get; set; }
    }

    [Verb("encryption", HelpText = "Encrypts or decrypts the configuration file")]
    class EncryptionArguments
    {
        [Option('a', "action", Required = true, HelpText = "Either 'encrypt' or 'decrypt'")]
        public EncryptionAction Action { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password to encrypt or decrypt the configuration files.")]
        public string Password { get; set; }

        [Option('c', "configuration", Required = false, HelpText = "File path to the configuration file. Defaults to execution folder.")]
        public string ConfigurationPath { get; set; }
    }
}
