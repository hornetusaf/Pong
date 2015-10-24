using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace PongServidor
{
    public partial class Form1 : Form
    {
        Graphics grafico;
        Tabla tabla1 = new Tabla() { X = 50, Y = 220, tamaño = new Size(10, 80), direccion_Y = 1, velocidad = 30 };
        Tabla tabla2 = new Tabla() { X = 722, Y = 220, tamaño = new Size(10, 80), direccion_Y = 1, velocidad = 1 };
        Tabla medio = new Tabla() { X = 390, Y = 0, tamaño = new Size(10, 600) };
        Bola mibola = new Bola() { X = 375, Y = 250, tamaño = new Size(20, 20), direccion_X = 1, direccion_Y = 1, velocidad = 10 };
        int cont1 = 0, cont2 = 0;

        public Form1()
        {
            InitializeComponent();
            grafico = CreateGraphics();
            DoubleBuffered = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            grafico.FillRectangle(Brushes.White, tabla1.rectangulo);
            grafico.FillRectangle(Brushes.White, tabla2.rectangulo);
            grafico.FillRectangle(Brushes.White, medio.rectangulo);

            grafico.FillEllipse(Brushes.Black, mibola.rectangulo);
            movimiento_bola();
            grafico.FillEllipse(Brushes.White, mibola.rectangulo);

            if (cont1 == 10)
            {
                label1.Text = "Ganador";
                cont1 = 0;
                cont2 = 0;
                label2.Text = cont2.ToString();
            }

            if (cont2 == 10)
            {
                label2.Text = "Ganador";
                cont1 = 0;
                cont2 = 0;
                label1.Text = cont1.ToString();
            }

            colision_pared();
            colision_tabla();

            send("224.5.6.7", "5000", "1", "2");
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            grafico.FillRectangle(Brushes.Black, tabla2.rectangulo);
            tabla2.Y = e.Y;
            grafico.FillRectangle(Brushes.White, tabla2.rectangulo);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            grafico.FillRectangle(Brushes.Black, tabla1.rectangulo);

            if (e.KeyCode == Keys.Up)
                tabla1.direccion_Y = 0;
            if (e.KeyCode == Keys.Down)
                tabla1.direccion_Y = 1;

            if (tabla1.direccion_Y == 1 && tabla1.Y < Height - 140)
                tabla1.Y += tabla1.velocidad;
            if (tabla1.direccion_Y == 0 && tabla1.Y > 20)
                tabla1.Y -= tabla1.velocidad;

            grafico.FillRectangle(Brushes.White, tabla1.rectangulo);
        }

        public void movimiento_bola()
        {
            if (mibola.direccion_X == 1)
                mibola.X += mibola.velocidad;
            if (mibola.direccion_X == 0)
                mibola.X -= mibola.velocidad;
            if (mibola.direccion_Y == 1)
                mibola.Y += mibola.velocidad;
            if (mibola.direccion_Y == 0)
                mibola.Y -= mibola.velocidad;
        }

        public void colision_pared()
        {
            if (mibola.X < -20)
            {
                mibola.direccion_X = 1;
                mibola.X = 390;
                mibola.Y = 290;
                cont2++;
                label2.Text = cont2.ToString();
            }
            if (mibola.X > Width)
            {
                mibola.direccion_X = 0;
                mibola.X = 390;
                mibola.Y = 290;
                cont1++;
                label1.Text = cont1.ToString();
            }

            if (mibola.Y < 15)
                mibola.direccion_Y = 1;
            if (mibola.Y > Height - 80)
                mibola.direccion_Y = 0;
        }

        public void colision_tabla()
        {
            if (mibola.direccion_X == 0 && mibola.rectangulo.IntersectsWith(tabla1.rectangulo))
                mibola.direccion_X = 1;
            if (mibola.direccion_X == 1 && mibola.rectangulo.IntersectsWith(tabla2.rectangulo))
                mibola.direccion_X = 0;
        }

        public void send(string mcastGroup, string port, string ttl, string rep)
        {
            IPAddress ip;
            try
            {
                Console.WriteLine("MCAST Send on Group: {0} Port: {1} TTL: {2}", mcastGroup, port, ttl);
                ip = IPAddress.Parse(mcastGroup);

                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));

                s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, int.Parse(ttl));

                byte[] data = new byte[128];
                data = Encoding.ASCII.GetBytes(tabla1.Y.ToString() + ',' + tabla2.Y.ToString() + ',' + mibola.X.ToString() + ',' + mibola.Y.ToString() + ',' + cont1.ToString() + ',' + cont2.ToString() + ',');
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(mcastGroup), int.Parse(port));
                Console.WriteLine("Connecting...");
                s.Connect(ipep);
                Console.WriteLine("Sending data");
                s.Send(data, data.Length, SocketFlags.None);
                Console.WriteLine("Closing Connection...");
                s.Close();
            }
            catch (System.Exception e) { Console.Error.WriteLine(e.Message); }
        }
    }
}