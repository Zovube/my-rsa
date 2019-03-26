using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace MyRSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void genKeys()
        {
            PrimeGenerator primeGenerator = new PrimeGenerator();
            BigInteger p = primeGenerator.genPrime();
            BigInteger q = primeGenerator.genPrime();
            while (p == q) q = primeGenerator.genPrime();
            richTextBox1.Text = p.ToString();
            richTextBox2.Text = q.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            genKeys();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Length == 0)
                genKeys();
            BigInteger p, q, exp;
            BigInteger.TryParse(richTextBox1.Text, out p);
            BigInteger.TryParse(richTextBox2.Text, out q);
            BigInteger phi = (p - 1) * (q - 1);
            BigInteger.TryParse(richTextBox3.Text, out exp);
            NumberTheory myRsa = new NumberTheory();
            if (myRsa.myGcd(exp, phi) != 1)
            {
                label4.Text = "GCD(E, PHI(N)) != 1 \n PLEASE SELECT ANOTHER E";
                label4.ForeColor = Color.Red;
                label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                label4.Visible = true;
                return;
            }
            else
            {
                label4.Text = "    GCD(E, PHI(N)) = 1";
                label4.ForeColor = Color.Green;
                label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                label4.Visible = true;
            }
            KeysPair keys = new KeysPair(p, q, exp);
            var publicKey = keys.getPublicKey();
            var privateKey = keys.getPrivateKey();

            richTextBox4.Text = "Public key : {" + publicKey.Item1.ToString() + ", " + publicKey.Item2.ToString() + " }";
            richTextBox5.Text = "Private key : {" + privateKey.Item1.ToString() + ", " + privateKey.Item2.ToString() + " }";
            richTextBox4.Visible = true;
            richTextBox5.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BigInteger text;
            BigInteger.TryParse(richTextBox6.Text, out text);
            BigInteger p, q, exp;
            BigInteger.TryParse(richTextBox1.Text, out p);
            BigInteger.TryParse(richTextBox2.Text, out q);
            BigInteger.TryParse(richTextBox3.Text, out exp);
            KeysPair keys = new KeysPair(p, q, exp);
            BigInteger cypherText = keys.encrypt(text);
            richTextBox7.Text = cypherText.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BigInteger cypherText;
            BigInteger.TryParse(richTextBox7.Text, out cypherText);
            BigInteger p, q, exp;
            BigInteger.TryParse(richTextBox1.Text, out p);
            BigInteger.TryParse(richTextBox2.Text, out q);
            BigInteger.TryParse(richTextBox3.Text, out exp);
            KeysPair keys = new KeysPair(p, q, exp);
            BigInteger text = keys.decrypt(cypherText);
            richTextBox8.Text = text.ToString();
        }

    }
}