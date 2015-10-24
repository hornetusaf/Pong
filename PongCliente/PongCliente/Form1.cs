using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace PongCliente
{
    public partial class Form1 : Form
    {
        Graphics grafico;
        Tabla tabla1 = new Tabla() { X = 50, Y = 220, tamaño = new Size(10, 80), direccion_Y = 1, velocidad = 30 };
        Tabla tabla2 = new Tabla() { X = 722, Y = 220, tamaño = new Size(10, 80), direccion_Y = 1, velocidad = 1 };
        Tabla medio = new Tabla() { X = 390, Y = 0, tamaño = new Size(10, 600) };
        Bola mibola = new Bola() { X = 375, Y = 250, tamaño = new Size(20, 20), direccion_X = 1, direccion_Y = 1, velocidad = 10 };
        int tabla1Y, tabla2Y, bolaX, bolaY,cont1, cont2;

        string mcastGroup = "224.5.6.7", port = "5000";
        Socket s;
        byte[] b = new byte[128];
        string str;

        public Form1()
        {
            InitializeComponent();
            grafico = CreateGraphics();
            DoubleBuffered = true;

            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, int.Parse(port));
            s.Bind(ipep);
            IPAddress ip = IPAddress.Parse(mcastGroup);
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Waiting for data..");
            s.Receive(b);
            str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
            string[] cadena = str.Split(',');
            tabla1Y = Convert.ToInt32(cadena[0]);
            tabla2Y = Convert.ToInt32(cadena[1]);
            bolaX = Convert.ToInt32(cadena[2]);
            bolaY = Convert.ToInt32(cadena[3]);
            cont1 = Convert.ToInt32(cadena[4]);
            cont2 = Convert.ToInt32(cadena[5]);

            Console.WriteLine("tabla1Y: " + tabla1Y);
            Console.WriteLine("tabla2Y: " + tabla2Y);
            Console.WriteLine("bolaX: " + bolaX);
            Console.WriteLine("bolaY: " + bolaY);
            Console.WriteLine("cont1: " + cont1);
            Console.WriteLine("cont2: " + cont2);
            grafico.FillRectangle(Brushes.White, medio.rectangulo);

            grafico.FillRectangle(Brushes.Black, tabla1.rectangulo);
            tabla1.Y = tabla1Y;
            grafico.FillRectangle(Brushes.White, tabla1.rectangulo);

            grafico.FillRectangle(Brushes.Black, tabla2.rectangulo);
            tabla2.Y = tabla2Y;
            grafico.FillRectangle(Brushes.White, tabla2.rectangulo);

            grafico.FillEllipse(Brushes.Black, mibola.rectangulo);
            mibola.X = bolaX;
            mibola.Y = bolaY;
            grafico.FillEllipse(Brushes.White, mibola.rectangulo);

            if (cont1 == 10)
            {
                label1.Text = "Ganador";
                cont1 = 0;
                cont2 = 0;
                label2.Text = cont2.ToString();
            }
            else
                label1.Text = cont1.ToString();


            if (cont2 == 10)
            {
                label2.Text = "Ganador";
                cont1 = 0;
                cont2 = 0;
                label1.Text = cont1.ToString();
            }
            else
                label2.Text = cont2.ToString();            
        }
    }
}
