using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


/*
 Please do also provide some thoughts (in textual form only) on how the following requirement could be implemented:
- it should provide information if variability constraints are violated 
	- Example: The parameter 'powerSupply' is required to be set to 'big' if the number of aisles in the sub-system config is >=5.

 Creating validation helper class for Configuration to check if any of the boundaries were violated.
 */
namespace Config
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new Configuration();
            ReadFile("Base_Config.txt", ref configuration);
            PrintConfig(configuration);
            ReadFile("Project_Config.txt", ref configuration);
            PrintConfig(configuration);
            RequestConfiguration(configuration);
        }

        //Read configurations from file
        public static void ReadFile(string fileName, ref Configuration configuration)
        {
            // Get all configurations from class
            var properties = configuration.GetType().GetProperties();
            var file = File.ReadAllLines(fileName);

            foreach (var line in file)
            {
                // Find line with configuration name and value
                if (Regex.IsMatch(line, @"^\w") && line.Contains(':'))
                {
                    // Remove unused characters
                    string parameterToParse = line.Remove(line.IndexOf('/'));
                    parameterToParse = parameterToParse.Replace("\t", "");
                    parameterToParse = parameterToParse.Replace(" ", "");
                    string[] values = parameterToParse.Split(":", 2);
                    // Find configuration which will be changed
                    var propToChange = properties.Where(p => p.Name.ToLower() == values[0].ToLower()).FirstOrDefault();
                    // Set required type of value in file
                    var valueToSet = Deserialize(values[1], propToChange.PropertyType, propToChange.Name);
                    propToChange.SetValue(configuration, valueToSet);
                }
            }

            Console.WriteLine("\nFile {0} loaded..", fileName);
        }

        //Write all current configuration into console
        public static void PrintConfig(Configuration config)
        {
            var properties = config.GetType().GetProperties();
            Console.WriteLine("\nCurrent configuration: ");
            foreach (var prop in properties)
            {
                try
                {
                    Console.WriteLine("{0} : {1}", prop.Name, prop.GetValue(config));
                }
                // Handle exceptions if configuration was not yet set up
                catch (NullReferenceException)
                { }
                catch (System.Reflection.TargetInvocationException)
                { }
            }
        }

        //Request single configuration element
        public static void RequestConfiguration(Configuration configuration)
        {
            var properties = configuration.GetType().GetProperties();
            Console.WriteLine("\nInsert configuration id..");
            //User input for configuration
            var requestedConfig = Console.ReadLine();
            var returnedConfig = properties.FirstOrDefault(p => p.Name == requestedConfig);
            if (returnedConfig != null)
            {
                try
                {
                    Console.WriteLine("{0} : {1}", returnedConfig.Name, returnedConfig.GetValue(configuration));
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    Console.WriteLine("{0} type is incorrect.", returnedConfig.Name);
                }  
            }
            else
            {
                //Configuration was not found
                Console.WriteLine("{0} : Error", requestedConfig);
            }
            //Repeat if pressed R
            Console.WriteLine("\nPress R to repeat or any key to quit..");
            if (Console.ReadKey().Key == ConsoleKey.R)
            {
                RequestConfiguration(configuration);
            }

        }
        //Deserialize string data into configuration
        private static object Deserialize(string input, Type toType, string configName)
        {
            try
            {
                if (toType == typeof(int))
                    return int.Parse(input);
                if (toType == typeof(InboudStrat))
                    return Enum.Parse(typeof(InboudStrat), input);
                if (toType == typeof(PowerSupp))
                    return Enum.Parse(typeof(PowerSupp), input);
                if (toType == typeof(TimeSpan))
                    return TimeSpan.Parse(input);

                throw new NotImplementedException(toType.ToString());
            }
            //Catch exceptions if input in configuration file is invalid.
            catch (FormatException)
            {
                Console.WriteLine("Exception resolving {0}. Value {1} is not of type {2}.", configName, input, toType.ToString());
                return null;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Exception resolving {0}. Value {1} is not of type {2}.", configName, input, toType.ToString());
                return null;
            }

        }
    }

    public class Configuration
    {
        #region full props
        private int _ordersPerHour;

        public int OrdersPerHour
        {
            get
            {
                // throw exception if the value was not set
                if (object.Equals(_ordersPerHour, default(int)))
                    throw new NullReferenceException();
                return _ordersPerHour;
            }
            set { _ordersPerHour = value; }
        }

        private int _orderLinesPerOrder;

        public int OrderLinesPerOrder
        {
            get
            {
                if (object.Equals(_orderLinesPerOrder, default(int)))
                    throw new NullReferenceException();
                return _orderLinesPerOrder;
            }
            set { _orderLinesPerOrder = value; }
        }

        private InboudStrat _inboundStrategy;

        public InboudStrat InboundStrategy
        {
            get
            {
                if (object.Equals(_inboundStrategy, default(InboudStrat)))
                    throw new NullReferenceException();
                return _inboundStrategy;
            }
            set { _inboundStrategy = value; }
        }
        private PowerSupp _powerSupp;

        public PowerSupp PowerSupply
        {
            get
            {
                if (object.Equals(_powerSupp, default(PowerSupp)))
                    throw new NullReferenceException();
                return _powerSupp;
            }
            set { _powerSupp = value; }
        }

        private TimeSpan _resultStartTime;

        public TimeSpan ResultStartTime
        {
            get
            {
                if (object.Equals(_resultStartTime, default(TimeSpan)))
                    throw new NullReferenceException();
                return _resultStartTime;
            }
            set { _resultStartTime = value; }
        }

        private int _resultInterval;

        public int ResultInterval
        {
            get
            {
                if (object.Equals(_resultInterval, default(int)))
                    throw new NullReferenceException();
                return _resultInterval;
            }
            set { _resultInterval = value; }
        }
        private int _numberOfAisles;

        public int NumberOfAisles
        {
            get
            {
                if (object.Equals(_numberOfAisles, default(int)))
                    throw new NullReferenceException();
                return _numberOfAisles;
            }
            set { _numberOfAisles = value; }
        }
        #endregion
    }
    #region enums
    public enum InboudStrat
    {
        none,
        random,
        optimized
    }

    public enum PowerSupp
    {
        none,
        normal,
        big
    }
    #endregion
}
