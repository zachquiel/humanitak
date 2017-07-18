using System;
using System.Linq;

namespace SmartAdminMvc.Helpers {
    public class ObjetoCfdi {
        public static string BuildCfdiBase(DateTime fecha, double subtotal, double descuento, string motivoDescuento,
            double total, string codigoPostal, string serie, string folio) {
            var desc = new DescriptorCfdi {
                Fecha = fecha,
                Subtotal = subtotal,
                Descuento = descuento,
                MotivoDescuento = motivoDescuento,
                Total = total,
                LugarExpedicion = codigoPostal,
                Serie = serie,
                Folio = folio
            };
            return desc.ToString();
        }

        public static string BuildEmployeeBase(string rfc, string nombre, double importe) {
            var emp = new DatosReceptor {
                Nombre = nombre,
                Rfc = rfc
            };
            var con = new ConceptosNomina {
                Importe = importe
            };
            return emp + con.ToString();
        }

        public static string BuildEmmiterData(string curp, string registroPatronal, string rfcPatron) {
//, string origenRecurso, double montoRecursoPropio ) {
            var em = new DatosEmisor {
                Curp = curp,
                RegistroPatronal = registroPatronal,
                RfcPatronOrigen = rfcPatron
                //OrigenRecurso = origenRecurso,
                //MontoRecursoPropio = montoRecursoPropio
            };
            return em.ToString();
        }

        public static string BuildPaymentComplement(string tipoNomina, DateTime fechaPago, DateTime fechaInicial,
            DateTime fechaFinal,
            double dias, double totalOtros, string emisor, string numEmpleado, string curp,
            string tipoRegimen, string numSs, string cveEstado, string sindicalizado, string departamento,
            string cuentaBancaria,
            string banco, DateTime fechaInicioLaboral, string antiguedad, string puesto, string tipoContrato,
            string tipoJornada,
            string periodicidad, double salarioBase, string riesgo, double sdi, string[] patrones, double[] porcentajes,
            double totalGravado, double totalExento, double totalSueldos, double totalJubilacion, double totalSeparacion,
            string[] percepcionesExtra, string datosJubilacion, string datosSeparacion, double totalImpuestosRetenidos,
            double totalOtrasDeducciones,
            string[] deducciones, string[] incapacidad, string otrosPagos) {
            var nomina = new DatosNomina {
                TipoNomina = tipoNomina,
                FechaPago = fechaPago,
                FechaInicialPago = fechaInicial,
                FechaFinalPago = fechaFinal,
                NumDiasPagados = dias,
                TotalPercepciones = totalSueldos + totalJubilacion + totalSeparacion,
                TotalDeducciones = totalImpuestosRetenidos + totalOtrasDeducciones,
                TotalOtrosPagos = totalOtros
            };
            var receptor = new DatosReceptorNomina {
                NumEmpleado = numEmpleado,
                Curp = curp,
                TipoRegimen = tipoRegimen,
                NumSeguridadSocial = numSs,
                ClaveEntFed = cveEstado,
                Sindicalizado = sindicalizado,
                Departamento = departamento,
                CuentaBancaria = cuentaBancaria,
                Banco = banco,
                FechaInicioRelLaboral = fechaInicioLaboral,
                Antiguedad = antiguedad,
                Puesto = puesto,
                TipoContrato = tipoContrato,
                TipoJornada = tipoJornada,
                PeriodicidadPago = periodicidad,
                SalarioBaseCotApor = salarioBase,
                RiesgoPuesto = riesgo,
                SalarioDiarioIntegrado = sdi,
                RfcLabora = patrones,
                PorcentajeTiempo = porcentajes
            };
            var totalPercepciones = new DatosPercepciones {
                TotalGravado = totalGravado,
                TotalExento = totalExento,
                TotalSueldos = totalSueldos,
                TotalJubilacionPensionRetiro = totalJubilacion,
                TotalSeparacionIndemnizacion = totalSeparacion
            };
            var totalDeducciones = new DatosDeducciones {
                TotalImpuestosRetenidos = totalImpuestosRetenidos,
                TotalOtrasDeducciones = totalOtrasDeducciones
            };
            var percepciones = percepcionesExtra.Aggregate("", (current, percepcion) => current + percepcion);
            var deduccionesf = deducciones.Aggregate("", (current, deduccion) => current + deduccion);
            var incapacidades = incapacidad.Aggregate("", (current, evento) => current + evento);
            var otros = otrosPagos.Aggregate("", (current, otro) => current + otro);
            return nomina + emisor + receptor + totalPercepciones + percepciones + datosJubilacion + datosSeparacion +
                   totalDeducciones + deduccionesf + incapacidades + otros;
        }

        public static string BuildPercepciones(string tipoPercepcion, string clave, string concepto,
            double importeGravado, double importeExento, int[] dias,
            string[] tipoHoras, int[] horasExtra, double[] importePagado) {
            var percepcion = new DatosPercepcionesExtra {
                TipoPercepcion = tipoPercepcion,
                Clave = clave,
                Concepto = concepto,
                ImporteGravado = importeGravado,
                ImporteExento = importeExento,
                Dias = dias,
                TipoHoras = tipoHoras,
                HorasExtra = horasExtra,
                ImportePagado = importePagado
            };
            return percepcion.ToString();
        }

        public static string BuildDeducciones(string tipoDeduccion, string clave, string concepto, double importe) {
            var deduccion = new DatosDeduccion {
                TipoDeduccion = tipoDeduccion,
                Clave = clave,
                Concepto = concepto,
                Importe = importe
            };
            return deduccion.ToString();
        }

        public static string BuildIncapacidad(int diasIncapacidad, string tipoIncapacidad, double importe) {
            var incapacidad = new DatosIncapacidad {
                DiasIncapacidad = diasIncapacidad,
                TipoIncapacidad = tipoIncapacidad,
                ImporteMonetario = importe
            };
            return incapacidad.ToString();
        }

        public static string BuildOtrosPagos(string tipo, string clave, string concepto, double importe, double subsidio,
            double saldo,
            int anyo, double remanente) {
            var otros = new DatosOtrosPagos {
                TipoOtroPago = tipo,
                Clave = clave,
                Concepto = concepto,
                Importe = importe,
                SubsidioCausado = subsidio,
                SaldoAFavor = saldo,
                Anyo = anyo,
                RemanenteSalFav = remanente
            };
            return otros.ToString();
        }
    }

    internal class DescriptorCfdi {
        public DateTime Fecha { get; set; }

        private static string FormaDePago => "En una sola exhibición";

        public double Subtotal { get; set; }
        public double Descuento { get; set; }
        public string MotivoDescuento { get; set; }

        private static string Moneda => "MXN";

        public double Total { get; set; }

        private static string TipoDeComprobante => "Egreso";

        private static string MetodoDePago => "NA";

        public string LugarExpedicion { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }

        public override string ToString() {
            return $"#01@|{Fecha:yyyy-MM-ddTHH:mm:ss}|{FormaDePago}|{Subtotal:F}|{Descuento:F}|{MotivoDescuento}|" +
                   $"{Moneda}|{Total:F}|{TipoDeComprobante}|{MetodoDePago}|{LugarExpedicion}|{Serie}|{Folio}|";
        }
    }

    internal class DatosReceptor {
        public string Rfc { get; set; }
        public string Nombre { get; set; }

        public override string ToString() {
            return $"#02@|{Rfc}|{Nombre}|";
        }
    }

    internal class ConceptosNomina {
        private static string Cantidad => "1";

        private static string Unidad => "ACT";

        private static string Descripcion => "Pago de nómina";

        public double Importe { get; set; }

        public override string ToString() {
            return $"#03@|{Cantidad}|{Unidad}|{Descripcion}|{Importe:F}|{Importe:F}|";
        }
    }

    internal class DatosNomina {
        public string TipoNomina { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaInicialPago { get; set; }
        public DateTime FechaFinalPago { get; set; }
        public double NumDiasPagados { get; set; }
        public double TotalPercepciones { get; set; }
        public double TotalDeducciones { get; set; }
        public double TotalOtrosPagos { get; set; }

        public override string ToString() {
            return $"#05N@|{TipoNomina}|{FechaPago:yyyy-MM-ddTHH:mm:ss}|{FechaInicialPago:yyyy-MM-ddTHH:mm:ss}|" +
                   $"{FechaFinalPago:yyyy-MM-ddTHH:mm:ss}|{NumDiasPagados:F3}|{TotalPercepciones:F}|{TotalDeducciones:F}|" +
                   $"{TotalOtrosPagos:F}|";
        }
    }

    internal class DatosEmisor {
        public string Curp { get; set; }
        public string RegistroPatronal { get; set; }
        public string RfcPatronOrigen { get; set; }

        public string OrigenRecurso => "IP";

        public double MontoRecursoPropio { get; set; }

        public override string ToString() {
            return $"#05E@|{Curp}|{RegistroPatronal}|{RfcPatronOrigen}|";
        }
    }

    internal class DatosReceptorNomina {
        public string NumEmpleado { get; set; }
        public string Curp { get; set; }
        public string TipoRegimen { get; set; }
        public string NumSeguridadSocial { get; set; }
        public string ClaveEntFed { get; set; }
        public string Sindicalizado { get; set; }
        public string Departamento { get; set; }
        public string CuentaBancaria { get; set; }
        public string Banco { get; set; }
        public DateTime FechaInicioRelLaboral { get; set; }
        public string Antiguedad { get; set; }
        public string Puesto { get; set; }
        public string TipoContrato { get; set; }
        public string TipoJornada { get; set; }
        public string PeriodicidadPago { get; set; }
        public double SalarioBaseCotApor { get; set; }
        public string RiesgoPuesto { get; set; }
        public double SalarioDiarioIntegrado { get; set; }
        public string[] RfcLabora { get; set; }
        public double[] PorcentajeTiempo { get; set; }

        public override string ToString() {
            var tms = "";
            for (var i = 0; i < RfcLabora.Length; i++)
                tms += "%" + RfcLabora[i] + "/" + PorcentajeTiempo[i].ToString("F");
            return $"#05R@|{NumEmpleado}|{Curp}|{TipoRegimen}|{NumSeguridadSocial}|{ClaveEntFed}|{Sindicalizado}|{Departamento}|" +
                   $"{CuentaBancaria}|{Banco}|{FechaInicioRelLaboral:yyyy-MM-ddTHH:mm:ss}|{Antiguedad}|{Puesto}|{TipoContrato}|" +
                   $"{TipoJornada}|{PeriodicidadPago}|{SalarioBaseCotApor:F}|{RiesgoPuesto}|{SalarioDiarioIntegrado:F}|{tms}|";
        }
    }

    internal class DatosPercepciones {
        public double TotalGravado { get; set; }
        public double TotalExento { get; set; }
        public double TotalSueldos { get; set; }
        public double TotalJubilacionPensionRetiro { get; set; }
        public double TotalSeparacionIndemnizacion { get; set; }

        public override string ToString() {
            return $"#06@|{TotalGravado:F}|{TotalExento:F}|{TotalSueldos:F}|{TotalJubilacionPensionRetiro:F}|{TotalSeparacionIndemnizacion:F}|";
        }
    }

    internal class DatosPercepcionesExtra {
        public string TipoPercepcion { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public double ImporteGravado { get; set; }
        public double ImporteExento { get; set; }
        public int[] Dias { get; set; }
        public string[] TipoHoras { get; set; }
        public int[] HorasExtra { get; set; }
        public double[] ImportePagado { get; set; }
        public decimal ValorMercado { get; set; }
        public decimal PrecioAlOtrogarse { get; set; }

        public override string ToString() {
            var ret = $"#07@|{TipoPercepcion}|{Clave}|{Concepto}|{ImporteGravado:F}|{ImporteExento:F}|";
            if (TipoPercepcion == "019") {
                var tms = "";
                for (var i = 0; i < Dias.Length; i++)
                    tms += "%" + Dias[i] + "/" + TipoHoras[i] + "/" + HorasExtra[i] + "/" +
                           ImportePagado[i].ToString("F");
                ret += $"{tms}|";
            }
            if (TipoPercepcion == "045")
                ret += $"{ValorMercado:F3}|{PrecioAlOtrogarse:F3}|";
            return ret;
        }
    }

    internal class DatosJubilacion {
        public double IngresoAcumulable { get; set; }
        public double IngresoNoAcumulable { get; set; }
        public double TotalUnaExhibicion { get; set; }
        public double TotalParcialidad { get; set; }
        public double MontoDiario { get; set; }

        public override string ToString() {
            return $"#07A@|{IngresoAcumulable:F}|{IngresoNoAcumulable:F}|{TotalUnaExhibicion:F}|{TotalParcialidad:F}|{MontoDiario:F}|";
        }
    }

    internal class DatosSeparacion {
        public double TotalPagado { get; set; }
        public int NumAnyosServicio { get; set; }
        public double UltimoSueldoMensOrd { get; set; }
        public double IngresoAcumulable { get; set; }
        public double IngresoNoAcumulable { get; set; }

        public override string ToString() {
            return $"#07B@|{TotalPagado:F}|{NumAnyosServicio}|{UltimoSueldoMensOrd:F}|{IngresoAcumulable:F}|{IngresoNoAcumulable:F}|";
        }
    }

    internal class DatosDeducciones {
        public double TotalImpuestosRetenidos { get; set; }
        public double TotalOtrasDeducciones { get; set; }

        public override string ToString() {
            return $"#08@|{TotalImpuestosRetenidos:F}|{TotalOtrasDeducciones:F}|";
        }
    }

    internal class DatosDeduccion {
        public string TipoDeduccion { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public double Importe { get; set; }

        public override string ToString() {
            return $"#09@|{TipoDeduccion}|{Clave}|{Concepto}|{Importe:F}|";
        }
    }

    internal class DatosIncapacidad {
        public int DiasIncapacidad { get; set; }
        public string TipoIncapacidad { get; set; }
        public double ImporteMonetario { get; set; }

        public override string ToString() {
            return $"#10@|{DiasIncapacidad}|{TipoIncapacidad}|{ImporteMonetario:F}|";
        }
    }

    internal class DatosOtrosPagos {
        public string TipoOtroPago { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public double Importe { get; set; }
        public double SubsidioCausado { get; set; }
        public double SaldoAFavor { get; set; }
        public int Anyo { get; set; }
        public double RemanenteSalFav { get; set; }

        public override string ToString() {
            return $"#11@|{TipoOtroPago}|{Clave}|{Concepto}|{Importe:F}|{SubsidioCausado:F}|{SaldoAFavor:F}|{Anyo}|{RemanenteSalFav:F}|";
        }
    }
}