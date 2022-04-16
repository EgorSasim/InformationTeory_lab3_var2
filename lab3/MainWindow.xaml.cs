using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const long P_MAX = 3000;
        public const int P_MIN = 3;
        ElGamal elGam = new ElGamal();
        public byte[] readenFile;
        public byte[] cipheredData;
        public byte[] aValue;
        public byte[] bValue;
        
        public MainWindow()
        {
            InitializeComponent();

            //MessageBox.Show(Convert.ToString( elGam.IsPrime(BigInteger.Parse("11111111111111111111111"))));
            //MessageBox.Show(Convert.ToString(elGam.GetNOD(322328, 122120)));
            
            
        }

        private void generatePBtn_Click(object sender, RoutedEventArgs e)
        {
           
            List<long> primes = elGam.getPrimes(P_MAX);
            long randPrimeNum = elGam.getRandPrimeNum(primes, P_MIN);
            pValueTxtBox.Text = Convert.ToString(randPrimeNum);

        }

        private void generatePrimeRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isOk = elGam.IsPrime(BigInteger.Parse(pValueTxtBox.Text));
                if (isOk)
                {
                    choosenRootTxtBox.Text = Convert.ToString(elGam.generatePrimeRoot(BigInteger.Parse(pValueTxtBox.Text)));
                }
                else
                {
                    MessageBox.Show("P is not prime");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect p");
            }

            
            
            //if (elGam.IsPrime(BigInteger.Parse(pValueTxtBox.Text))) 
            //    choosenRootTxtBox.Text = Convert.ToString(elGam.generatePrimeRoot(BigInteger.Parse(pValueTxtBox.Text)));
            //else
            //{
            //    MessageBox.Show("P is not prime");
            //}
        }

        private void findPrimeRootsBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                bool isOk = elGam.IsPrime(BigInteger.Parse(pValueTxtBox.Text));
                if (isOk)
                {
                    List<BigInteger> primeRoots = elGam.findPrimeRoots(BigInteger.Parse(pValueTxtBox.Text));
                    MessageBox.Show(string.Join(",", primeRoots));
                    primeRootsListBox.ItemsSource = primeRoots;
                    rootCntTxtBox.Text = Convert.ToString(primeRoots.Count);
                }
                else
                {
                    MessageBox.Show("P is not prime");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect p");
            }
            
            //if (pValueTxtBox.Text == "")
            //{
            //    MessageBox.Show("An empty P value.");
            //}
            //else
            //{
            //    if (elGam.IsPrime(BigInteger.Parse(pValueTxtBox.Text)))
            //    {
            //        List<BigInteger> primeRoots = elGam.findPrimeRoots(BigInteger.Parse(pValueTxtBox.Text));
            //        MessageBox.Show(string.Join(",", primeRoots));
            //        primeRootsListBox.ItemsSource = primeRoots;
            //        rootCntTxtBox.Text = Convert.ToString(primeRoots.Count);
            //    }
            //    else
            //    {
            //        MessageBox.Show("P is not prime");
            //    }
            //}

            


        }

        private void primeRootsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            choosenRootTxtBox.Text = Convert.ToString( primeRootsListBox.SelectedItem );
        }

        private void generateClosedKeyBtn_Click(object sender, RoutedEventArgs e)
        {
           
            BigInteger value = elGam.getRandomBigInt(new BigInteger(2), BigInteger.Subtract(BigInteger.Parse(pValueTxtBox.Text), 1));
            closedKeyTextBox.Text = Convert.ToString(value);
        }


        private void generateYBtn_Click(object sender, RoutedEventArgs e)
        {
            yTxtBox.Text = Convert.ToString( elGam.FastExprModulo(BigInteger.Parse(choosenRootTxtBox.Text), BigInteger.Parse(closedKeyTextBox.Text), BigInteger.Parse(pValueTxtBox.Text)) );
        }

        private void generateKSecretNumBtn_Click(object sender, RoutedEventArgs e)
        {
            BigInteger k;
            do
            {
                k = elGam.getRandomBigInt(new BigInteger(2), BigInteger.Subtract(BigInteger.Parse(pValueTxtBox.Text), 1));
            } while (elGam.GetNOD(k, BigInteger.Subtract(BigInteger.Parse(pValueTxtBox.Text), 1)) != 1);

            kSecretNumTxtBox.Text = Convert.ToString(k);
        }

        private void openFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                fileContentTxtBox.Text = string.Join(" ", File.ReadAllBytes(openFileDialog.FileName));
                readenFile = File.ReadAllBytes(openFileDialog.FileName);
            }
                
        }

        private void generateABtn_Click(object sender, RoutedEventArgs e)
        {
            aValueTxtBox.Text = Convert.ToString(elGam.FastExprModulo( BigInteger.Parse(choosenRootTxtBox.Text), BigInteger.Parse(kSecretNumTxtBox.Text), BigInteger.Parse(pValueTxtBox.Text) ));
            
        }

        private void generateBBtn_Click(object sender, RoutedEventArgs e)
        {
            if(fileContentTxtBox.Text != "")
            {
                bValueTxtBox.Text = "";
                for (long i = 0; i < readenFile.Length; i++)
                {
                    bValueTxtBox.AppendText( (elGam.FastExprModuloB(BigInteger.Parse(yTxtBox.Text), BigInteger.Parse(kSecretNumTxtBox.Text), BigInteger.Parse(pValueTxtBox.Text), readenFile[i])).ToString() + "\n");
                }
            }
            else
            {
                MessageBox.Show("At first open file, please.");
            }
           
            
        }

        private void writeToCipheredFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if(BigInteger.Compare(BigInteger.Parse(closedKeyTextBox.Text) ,1) == 0)
            {
                MessageBox.Show("Incorrect X");
            }
            
            if(  (BigInteger.Compare( BigInteger.Parse(closedKeyTextBox.Text), BigInteger.Parse(pValueTxtBox.Text) ) < 0) ) 
            {
                cipheredFileContentTxtBox.Text = "";
                for (int i = 0; i < bValueTxtBox.LineCount - 1; i++)
                {
                    cipheredFileContentTxtBox.AppendText(aValueTxtBox.Text + "," + bValueTxtBox.GetLineText(i));
                }
            }
            else
            {
                MessageBox.Show("Incorrect X value");
            }

            

        }

        private void saveCipheredText_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    

                    myStream.Close();
                }
                File.WriteAllBytes(saveFileDialog.FileName, Encoding.ASCII.GetBytes(cipheredFileContentTxtBox.Text));
            }
        }



        ///////////////////////////////////////////////////////// DECRIPTION ////////////////////////////////////////

        public string[] readenCipheredFileCopy;
        
        public byte[] decriptedFile;
        private void openCipheredFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                cipheredFileContentDecriptTxtBox.Text = File.ReadAllText(openFileDialog.FileName);//string.Join(" ", File.ReadAllBytes(openFileDialog.FileName));
                readenCipheredFileCopy = cipheredFileContentDecriptTxtBox.Text.Split('\n',',');
                
            }

           
            
        }

        private void decriptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decipherFileContentTxtBox.Text = "";
                BigInteger p = BigInteger.Parse(pDecriptValueTxtBox.Text);
                BigInteger x = BigInteger.Parse(xDecriptValueTxtBox.Text);
                decriptedFile = new byte[readenCipheredFileCopy.Length / 2];
                for (long j = 0, i = 0; j < readenCipheredFileCopy.Length - 1; i++, j += 2)
                {
                    decriptedFile[i] = (byte)(BigInteger.Remainder(BigInteger.Multiply(BigInteger.Parse(readenCipheredFileCopy[j + 1]), BigInteger.Pow(BigInteger.Parse(readenCipheredFileCopy[j]), (int)BigInteger.Multiply(x, BigInteger.Subtract(p, 2)))), p));
                    decipherFileContentTxtBox.AppendText( decriptedFile[i].ToString());
                    
                }
            }
            catch {
                MessageBox.Show("Smth goes wrong");
            }
            
        }

        private void saveDecriptedFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {


                    myStream.Close();
                }
                File.WriteAllBytes(saveFileDialog.FileName, decriptedFile);
            }
        }
    }
}
