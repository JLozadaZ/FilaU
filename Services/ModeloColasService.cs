namespace FilaU.Services
{
    // Modelo para devolver todos los resultados del análisis de colas
    public class ResultadosModeloColas
    {
        public double Lambda { get; set; }           // Tasa de llegada (λ)
        public double Mu { get; set; }               // Tasa de servicio por servidor (μ)
        public int Servidores { get; set; }          // Número de servidores (s)
        public double FactorUtilizacion { get; set; }// Porcentaje de ocupación del sistema (ρ)
        public double P0 { get; set; }               // Probabilidad de que el sistema esté totalmente vacío
        public double Lq { get; set; }               // Número promedio de clientes en la COLA
        public double L { get; set; }                // Número promedio de clientes en el SISTEMA (cola + atendiendo)
        public double Wq { get; set; }               // Tiempo promedio de espera en COLA (en minutos)
        public double W { get; set; }                // Tiempo promedio total en el SISTEMA (en minutos)
        public bool EsEstable { get; set; }          // Indica si el sistema es estable (λ < s * μ)
    }

    public class ModeloColasService
    {
        /// <summary>
        /// Calcula las métricas de Investigación de Operaciones para un sistema M/M/s.
        /// Por defecto usa los datos del proyecto: lambda=18, mu=12, s=2.
        /// </summary>
        public ResultadosModeloColas CalcularMMs(double lambda = 18, double mu = 12, int s = 2)
        {
            var resultados = new ResultadosModeloColas
            {
                Lambda = lambda,
                Mu = mu,
                Servidores = s
            };

            // 1. Capacidad total de atención del sistema (s * μ)
            double capacidadTotal = s * mu;

            // 2. Factor de utilización (ρ = λ / (s * μ))
            // Indica el porcentaje promedio de ocupación de los servidores.
            resultados.FactorUtilizacion = lambda / capacidadTotal;

            // Validación de estabilidad del sistema:
            // Si la tasa de llegada (λ) supera o iguala la capacidad total (s * μ),
            // la fila crecería al infinito y el sistema colapsaría.
            if (resultados.FactorUtilizacion >= 1.0)
            {
                resultados.EsEstable = false;
                return resultados;
            }

            resultados.EsEstable = true;

            // 3. Cálculo de P0 (Probabilidad de que NO haya nadie en la cafetería)
            // Fórmula M/M/s:
            // P0 = 1 / [ Σ_{n=0}^{s-1} ((λ/μ)^n / n!) + ((λ/μ)^s / (s! * (1 - ρ))) ]
            double r = lambda / mu; // Relación λ/μ
            double suma = 0;

            // Sumatoria desde n = 0 hasta s - 1
            for (int n = 0; n < s; n++)
            {
                suma += Math.Pow(r, n) / Factorial(n);
            }

            // Término final para n = s
            double terminoS = (Math.Pow(r, s) / Factorial(s)) * (1.0 / (1.0 - resultados.FactorUtilizacion));

            // Inverso de la suma total
            resultados.P0 = 1.0 / (suma + terminoS);

            // 4. Lq: Número promedio de personas en la COLA (esperando físicamente o en turno)
            // Lq = [ P0 * (λ/μ)^s * ρ ] / [ s! * (1 - ρ)^2 ]
            double numeradorLq = resultados.P0 * Math.Pow(r, s) * resultados.FactorUtilizacion;
            double denominadorLq = Factorial(s) * Math.Pow(1.0 - resultados.FactorUtilizacion, 2);
            resultados.Lq = numeradorLq / denominadorLq;

            // 5. Wq: Tiempo promedio de espera en la COLA (en HORAS)
            // Wq = Lq / λ
            double wqHoras = resultados.Lq / lambda;
            // Convertimos las horas a MINUTOS para mostrarlo claramente en la web
            resultados.Wq = wqHoras * 60;

            // 6. W: Tiempo promedio total en el SISTEMA (Espera en cola + Tiempo de atención)
            // W = Wq + (1 / μ)  [en horas]
            double wHoras = wqHoras + (1.0 / mu);
            resultados.W = wHoras * 60; // Convertido a minutos

            // 7. L: Número promedio de clientes en el SISTEMA completo
            // L = λ * W
            resultados.L = lambda * wHoras;

            return resultados;
        }

        // Función auxiliar para calcular factoriales (n!)
        private double Factorial(int n)
        {
            if (n <= 1) return 1;
            double resultado = 1;
            for (int i = 2; i <= n; i++)
            {
                resultado *= i;
            }
            return resultado;
        }
    }
}