using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

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

        public static void ReadFile(string fileName, ref Configuration configuration)
        {
            var properties = configuration.GetType().GetProperties();
            var file = File.ReadAllLines(fileName);

            foreach (var line in file)
            {
                if (Regex.IsMatch(line, @"^\w") && line.Contains(':'))
                {
                    string parameterToParse = line.Remove(line.IndexOf('/'));
                    parameterToParse = parameterToParse.Replace("\t", "");
                    parameterToParse = parameterToParse.Replace(" ", "");
                    string[] values = parameterToParse.Split(":", 2);
                    var propToChange = properties.Where(p => p.Name.ToLower() == values[0].ToLower()).FirstOrDefault();
                    var valueToSet = Deserialize(values[1], propToChange.PropertyType);
                    propToChange.SetValue(configuration, valueToSet);
                }
            }

            Console.WriteLine("\nFile {0} loaded..", fileName);
        }

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

                catch (NullReferenceException)
                {
                    Console.WriteLine("{0} : Not configured", prop.Name);
                }
                catch(System.Reflection.TargetInvocationException)
                { }
            }
        }

        public static void RequestConfiguration(Configuration configuration)
        {
            var properties = configuration.GetType().GetProperties();
            Console.WriteLine("\nInsert configuration id..");
            var requestedConfig = Console.ReadLine();
            var returnedConfig = properties.FirstOrDefault(p => p.Name == requestedConfig);
            if(returnedConfig != null)
            {
                Console.WriteLine("{0} : {1}", returnedConfig.Name, returnedConfig.GetValue(configuration));
            }
            else
            {
                Console.WriteLine("{0} : Error", requestedConfig);
            }
            Console.WriteLine("\nPress R to repeat or any key to quit..");
            if(Console.ReadKey().Key == ConsoleKey.R)
            {
                RequestConfiguration(configuration);
            }

        }

        private static object Deserialize(string input, Type toType)
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
    }

    public class Configuration
    {
        #region full props
        private int _ordersPerHour;

        public int OrdersPerHour
        {
            get
            {
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
