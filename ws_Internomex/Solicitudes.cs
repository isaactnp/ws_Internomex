using Internomex.Models.BaseDeDatos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ws_Internomex.Model;

namespace ws_Internomex
{
   public class Solicitudes : IHostedService, IDisposable
    {
        private readonly ILogger<Solicitudes> _logger;
        private Timer _timerIniciar;

        //public string Urls = "https://prueba.gphsis.com/sisfusion/API/Comisiones/index_get";

        public string Urls = "https://prueba.gphsis.com/sisfusion/";
        public Solicitudes(ILogger<Solicitudes> logger)
        {
            _logger = logger;
          
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Inicia Servicio ", DateTime.Now);
                Log.Logger.Information("Inicia Servicio ", DateTime.Now);
             
                Internomex.Models.BaseDeDatos.InternomexServiceContext serviceContext = new Internomex.Models.BaseDeDatos.InternomexServiceContext();
                Internomex.Models.BaseDeDatos.Parametro parametros = serviceContext.Parametros.Where(x => x.IdParametro == 1).FirstOrDefault();
                if (parametros == null)
                {
                    throw new ArgumentException("No se encuentra configurado el parametro");
                }
                SolicitudesDeComision(parametros.Valor);


            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error al cargar configuración");
                _logger.LogError("Inicia Servicio ", ex);
            }
            return Task.CompletedTask;
        }

        private void SolicitudesDeComision(string proceso)
        {
            try
            {
                _logger.LogInformation("Actualización de solicitudes de comision " + DateTime.Now);
                Log.Logger.Information("Actualización de solicitudes de comision " + DateTime.Now);
                TimeSpan timeSpan = new TimeSpan(0, Convert.ToInt32(proceso), 0);
                TimeSpan time = new TimeSpan();
                time = new TimeSpan(0, Convert.ToInt32(proceso), 0);
                //time = new TimeSpan(0, 3, 0);

                _logger.LogInformation("Actualizacion de solicitudes de comision, tiempo para proxima ejecución en minutos " + time.TotalMinutes);
                Log.Logger.Information("Actualizacion de solicitudes de comision, tiempo para proxima ejecución en minutos " + time.TotalMinutes);
                _logger.LogInformation("Actualizacion de solicitudes de comision, tiempo entre ejecución en minutos " + timeSpan.TotalMinutes);
                Log.Logger.Information("Actualizacion de solicitudes de comision, tiempo entre ejecución en minutos " + timeSpan.TotalMinutes);
                _timerIniciar = new Timer(ActualizacionDeSolicitudes, null, time, timeSpan);
                //ActualizacionDeSolicitudes();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error al configurar la mensualidad envío automático de factura ", ex);
            }
        }

        private void ActualizacionDeSolicitudes(object state)
        {
            try
            {
             
                    using (InternomexServiceContext serviceContext = new InternomexServiceContext())
                    {
                    IQueryable<SolicitudDeComision> IQSolicitudcomision = serviceContext.SolicitudesDeComision;
                    SolicitudDeComision solicitudDeComision = null;
                    List<SolicitudDeComision> ListSolicitudes = new List<SolicitudDeComision>();
                    //SolicitudDeComision ListSolicitudes = new SolicitudDeComision();

                    Model.Rootobject jsonRoot = new Rootobject();
                    string res = "";

                    using (var client = new HttpClient())
                    {
                        try
                        {

                            client.BaseAddress = new Uri(Urls);
                            client.DefaultRequestHeaders.Accept.Clear();                          
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            var response = client.GetAsync("API/Comisiones/index_get").Result;
                            //HttpResponseMessage response = client.GetAsync(Urls + "Api/Comisiones/index_get").Result;
                            if (response.IsSuccessStatusCode)
                            {
                               
                                using (HttpContent content = response.Content)
                                {
                                    Task<string> result = content.ReadAsStringAsync();
                                    res = result.Result;
                                    jsonRoot = JsonConvert.DeserializeObject<Model.Rootobject>(res);
                                  
                                    foreach (var item in jsonRoot.RESULT.COMDATA)
                                    {
                                        SolicitudDeComision solicitudes = serviceContext.SolicitudesDeComision.Where(x => x.IdSolicitudDeComision ==Convert.ToInt32(item.idSolicitudDeComision) && x.IdPago == Convert.ToInt32(item.idPago)).FirstOrDefault();
                                       
                                            if (solicitudes == null)
                                            {
                                                solicitudDeComision = new SolicitudDeComision();
                                                solicitudDeComision.IdSolicitudDeComision = Convert.ToInt32(item.idSolicitudDeComision);
                                                solicitudDeComision.FechaSolicitudDeComision = Convert.ToDateTime(item.fechaSolicitudDeComision);
                                                solicitudDeComision.NombreAsesor = item.nombreComisionista;
                                                solicitudDeComision.IdEmpleado = Convert.ToString(item.rfc);
                                                solicitudDeComision.Puesto = item.puesto;
                                                solicitudDeComision.Cliente = item.cliente;
                                                solicitudDeComision.Desarrollo = item.desarrollo;
                                                solicitudDeComision.Cluster = item.cluster;
                                                solicitudDeComision.IdLote = string.Empty;
                                                solicitudDeComision.Lote = item.lote;
                                                solicitudDeComision.Superficie = item.superficie;
                                                solicitudDeComision.PrecioNeto = Convert.ToDecimal(item.precioNeto);
                                                solicitudDeComision.TotalComision = Convert.ToDecimal(item.totalComision);
                                                solicitudDeComision.PorcentajeComision = Convert.ToDecimal(item.porcentajeComision);
                                                solicitudDeComision.EntradaVenta = item.entradaVenta;
                                                solicitudDeComision.Esquema = item.esquema;
                                                solicitudDeComision.Cobro = item.cobro;
                                                solicitudDeComision.XML = item.xml;
                                                solicitudDeComision.IdPago = Convert.ToInt32(item.idPago);
                                                solicitudDeComision.FechaHoraCaptura = DateTime.Now;
                                                solicitudDeComision.Estatus = "Pendiente por aprobar";
                                                ListSolicitudes.Add(solicitudDeComision);
                                               
                                            }                                      
                                      
                                    }

                                }
                            }
                            else
                            {
                                using (HttpContent content = response.Content)
                                {
                                    Task<string> result = content.ReadAsStringAsync();
                                    throw new ArgumentException(result.Result);
                                }
                            }
                          

                        }
                        catch (Exception ex)
                        {

                            throw new ArgumentException("Error al Actualizar", ex);
                        }
                    }

                    serviceContext.SolicitudesDeComision.AddRange(ListSolicitudes);
                    serviceContext.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "comisiones");
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timerIniciar?.Change(Timeout.Infinite, 0);         
            Log.Logger.Information("FinalizaServicio ", DateTime.Now);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            Log.Logger.Information("DisposeServicio ", DateTime.Now);
            _timerIniciar?.Dispose();
          
        }

    }
}
