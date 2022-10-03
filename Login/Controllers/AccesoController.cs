using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Login.Models; //Usar modelo dentro del controlador
using System.Data.SqlClient;
using System.Data;

namespace Login.Controllers
{
    public class AccesoController : Controller
    {

        //Cadena de conexion a la BD

        static string Conexion = "Data Source =DESKTOP-NCQFDV0\\SQLEXPRESS; Initial Catalog = DB_Access; Integrated Security = True";


        // GET: Acceso
        public ActionResult Login() //Formulario
        {
            return View();
        }

        public ActionResult Registrar() //Conexion a la BD
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if(oUsuario.Contrasena == oUsuario.ConfContrasena)
            {
                //Actualizando la propiedad "Contrasena" con la contraseña encriptada
                oUsuario.Contrasena = EncriptarContrasena(oUsuario.Contrasena);
            }
            else
            {
                ViewData["Mensaje"] = "Las Contraseñas No Coinciden"; //ViewData envia datos del controlador a la vista
                return View();
            }


            //Conexion a la BD
            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                SqlCommand comando = new SqlCommand("sp_RegistrarUsuario", connection);
                comando.Parameters.AddWithValue("Correo", oUsuario.Correo);
                comando.Parameters.AddWithValue("Contrasena", oUsuario.Contrasena);    
                comando.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                comando.Parameters.Add("Mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                comando.CommandType = CommandType.StoredProcedure;

                connection.Open();
                comando.ExecuteNonQuery();

                registrado = Convert.ToBoolean(comando.Parameters["Registrado"].Value);
                mensaje = comando.Parameters["Mensaje"].Value.ToString();
            }
            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(Usuario oUsuario) //Formulario
        {

            oUsuario.Contrasena = EncriptarContrasena(oUsuario.Contrasena);

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                SqlCommand comando = new SqlCommand("sp_Validarusuario", connection);
                comando.Parameters.AddWithValue("Correo", oUsuario.Correo);
                comando.Parameters.AddWithValue("Contrasena", oUsuario.Contrasena);
                comando.CommandType = CommandType.StoredProcedure;

                connection.Open();
               oUsuario.IdUsuario = Convert.ToInt32(comando.ExecuteScalar().ToString()); //Leer primera fila y primera columna
            }

            if (oUsuario.IdUsuario != 0)
            {
                Session["usuario"] = oUsuario; //Almacenar todo el objeto oUsuario
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario No Encontrado";
                return View();
            }
        }

        //Encriptar Contraseña SHA256
        public static string EncriptarContrasena(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}