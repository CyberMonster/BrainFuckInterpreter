using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BrainFuckInterpreter
{
    public class BFInterpreter
    {
        private void ExceptionHandler()
        {
            var CTC = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.DarkRed;
            System.Console.WriteLine("--!> Time left for task!");
            System.Console.ForegroundColor = CTC;
            DataOut(new string[0]);
            this.MainThread.Abort();
        }
        public void ElapsedEventHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.ExceptionHandler();
            //throw new Exception("Time left for task!");
        }
        public BFInterpreter(string[] args, System.Threading.Thread MainThread)
        {
            //args = new string[1] { @":!{20}200000:++++++++++++++++++++++++++++++++++++++++++++++++>>><<<[->+>+>+<<<],>[-<->],>>[-<<->>]<<<<<>>[-<+<+>>]>[->>+>+<<<]<<[->>>>[->>+<<]>[-<<<<+>>>>]>[-<+<+>>]<<<<<<]>>>[-<+>]>[-<<<<+>>>>]>[-]" };

            this.MainThread = MainThread;

            try
            {
                this.Settings = string.Concat(args[0].ToList().GetRange(args[0].IndexOf(':') + 1, args[0].IndexOf(':', args[0].IndexOf(':') + 1) - args[0].IndexOf(':') - 1));
            }
            catch
            {
                this.Settings = "";
            }

            this.MaxListCount = -1;
            try
            {
                var t = Settings.ToList().GetRange(Settings.IndexOf('{') + 1, Settings.IndexOf('}') - Settings.IndexOf('{') - 1);
                this.MaxListCount = int.Parse(string.Concat(Settings.ToList().GetRange(Settings.IndexOf('{') + 1, Settings.IndexOf('}') - Settings.IndexOf('{') - 1).Where(x => "0123456789".Where(y => x == y).Count() > 0)));
                this.Settings = this.Settings.Remove(Settings.IndexOf('{'), Settings.IndexOf('}') - Settings.IndexOf('{') + 1);
            }
            catch
            {
                ;
            }
            this.DataTape = new List<byte>();
            for (var i = 0; i < (this.MaxListCount < 1 ? 1 : this.MaxListCount); ++i)
            {
                this.DataTape.Add(0);
            }
            this.DataPointerIndex = 0;
            this.DataPointer = 0;
            
            BFInt(args);

            DataOut(args);

            //System.Console.ReadKey();
        }
        private void DataOut(string[] args, string SpecialMsg = "Program finished!")
        {
            try
            {
                System.Console.WriteLine(SpecialMsg);
                System.Console.WriteLine("Program: {0}", string.Concat(args));
                System.Console.Write("Tape: ");
                byte buffer;
                for (var i = 0; i < this.DataTape.Count(); ++i)
                {
                    buffer = this.DataTape[i];
                    System.Console.Write("{0}{1}{2}", i == this.DataPointerIndex ? "-->" : "", buffer, "; ");
                }
                System.Console.WriteLine();
                //string.Concat(this.DataTape.Select((x, i) => string.Concat(i == this.DataPointerIndex ? "-->" : "", x.ToString(), "; "))));
                System.Console.WriteLine("Position: {0}", this.DataPointerIndex);
            }
            catch
            {
                System.Console.WriteLine("Position: {0}", this.DataPointerIndex);
            }
        }
        private void DataOut(string args, string SpecialMsg = "Program finished!", int Position = -1)
        {
            try
            {
                System.Console.WriteLine(SpecialMsg);
                System.Console.WriteLine("Program: {0}", string.Concat(args.Select((x, i) => string.Concat(i == Position ? "~~~~%" : "", x))));
                System.Console.Write("Tape: ");
                byte buffer;
                for (var i = 0; i < this.DataTape.Count(); ++i)
                {
                    buffer = this.DataTape[i];
                    System.Console.Write("{0}{1}{2}", i == this.DataPointerIndex ? "-->" : "", buffer, "; ");
                }
                //string.Concat(this.DataTape.Select((x, i) => string.Concat(i == this.DataPointerIndex ? "-->" : "", x.ToString(), "; "))));
                System.Console.WriteLine("Position: {0}", this.DataPointerIndex);
            }
            catch
            {
                System.Console.WriteLine("Position: {0}", this.DataPointerIndex);
            }
        }
        private void BFInt(string[] CommandsList)
        {
            this.Timer = new System.Timers.Timer();
            if (this.Settings.Where(x => x == '!').Count() >= 1)
            {
                this.Timer.Enabled = true;
                this.Timer.AutoReset = false;
                this.Timer.Interval = Double.Parse(string.Concat(this.Settings.Where(x => "0123456789".Where(y => y == x).Count() > 0).Count() > 0 ? this.Settings.Where(x => "0123456789".Where(y => y == x).Count() > 0) : "1000"));
                this.Timer.Elapsed += ElapsedEventHandler;
            }
            foreach (string Command in CommandsList)
            {
                if (this.BFTestCommand(Command))
                {
                    this.Timer.Start();
                    BFRun(Command);
                    this.Timer.Stop();
                }
            }
        }
        private bool BFTestCommand(string Command)
        {
            return true;
        }
        private void BFRun(string Command)
        {
            char Operation;
            int BracketsCount = 0;
            
            for (var Counter = 0; Counter < Command.Length; ++Counter)
            {
                if (this.DataTape.Count() > this.MaxListCount && this.MaxListCount != -1 && this.Settings.Where(x => char.ToLower(x) == 'f').Count() > 0)
                {
                    this.ExceptionHandler();
                }
                Operation = Command[Counter];
                switch (Operation)
                {
                    case '<':
                        if (this.DataPointerIndex == 0)
                        {
                            if (this.Settings.Where(x => x == '\\').Count() > 0)
                            {
                                this.ExceptionHandler();
                            }
                            else
                            {
                                this.DataTape.Insert(0, 0);
                                ++this.DataPointerIndex;
                            }
                        }
                        --this.DataPointerIndex;
                        break;
                    case '>':
                        ++this.DataPointerIndex;
                        if (this.DataPointerIndex >= this.DataTape.Count)
                        {
                            if (this.Settings.Where(x => x == '/').Count() > 0)
                            {
                                this.ExceptionHandler();
                            }
                            else
                            {
                                this.DataTape.Add(0);
                            }
                        }
                        break;
                    case '+':
                        ++this.DataTape[this.DataPointerIndex];
                        break;
                    case '-':
                        --this.DataTape[this.DataPointerIndex];
                        break;
                        ///Set in data pointer
                    case '.':
                        this.Timer.Stop();
                        this.DataPointer = this.DataTape[this.DataPointerIndex];
                        System.Console.Write(char.ConvertFromUtf32(this.DataPointer));
                        this.Timer.Start();
                        break;
                        ///Set in data tape
                    case ',':
                        this.Timer.Stop();
                        System.Console.Write("Input any char: \n");
                        this.DataPointer = (byte)System.Console.ReadKey().KeyChar;
                        System.Console.Write('\n');
                        //System.Console.WriteLine();
                        this.DataTape[this.DataPointerIndex] = this.DataPointer;
                        this.Timer.Start();
                        break;
                    case '[':
                        if (this.DataTape[this.DataPointerIndex] == 0)
                        {
                            this.DataTapeDump = this.DataTape;
                            int CountOfOpeningParts = 0;
                            for (var i = Counter ; i < Command.Length; ++i)
                            {
                                if (Command[i] == '[')
                                {
                                    ++CountOfOpeningParts;
                                }
                                else
                                {
                                    if (Command[i] == ']')
                                    {
                                        --CountOfOpeningParts;
                                    }
                                }
                                if (CountOfOpeningParts == 0)
                                {
                                    Counter = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            ++BracketsCount;
                        }
                        break;
                    case ']':
                        if (this.DataTape[this.DataPointerIndex] != 0)
                        {
                            int CountOfOpeningParts = 0;
                            for (var i = Counter; i >= 0; --i)
                            {
                                if (Command[i] == ']')
                                {
                                    ++CountOfOpeningParts;
                                }
                                else
                                {
                                    if (Command[i] == '[')
                                    {
                                        --CountOfOpeningParts;
                                    }
                                }
                                if (CountOfOpeningParts == 0)
                                {
                                    Counter = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            --BracketsCount;
                        }
                        break;
                    case '=':
                        this.Timer.Stop();
                        DataOut(Command, "DataOut", Counter);
                        System.Console.ReadKey();
                        this.Timer.Start();
                        break;
                    default:
                        break;
                }
            }
        }
        private List<byte> DataTape;
        private List<byte> DataTapeDump;
        private int DataPointerIndex;
        private byte DataPointer;
        private string Settings;
        private System.Timers.Timer Timer;
        private System.Threading.Thread MainThread;
        private int MaxListCount;
    }
}