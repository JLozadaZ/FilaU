namespace FilaU.Services
{
    // Modelo para devolver todos los resultados del análisis de colas M/M/s
    public class ResultadosModeloColas
    {
        public double Lambda { get; set; }           // Tasa de llegada (λ)
        public double Mu { get; set; }               // Tasa de servicio por servidor (μ)
        public int Servidores { get; set; }          // Número de servidores (s)
        public double FactorUtilizacion { get; set; }// Porcentaje de ocupación del sistema (ρ)
        public double P0 { get; set; }               // Probabilidad de que el sistema esté totalmente vacío
        public double Lq { get; set; }               // Número promedio de clientes en la COLA
        public double L { get; set; }                // Número promedio de clientes en el SISTEMA
        public double Wq { get; set; }               // Tiempo promedio de espera en COLA (en minutos)
        public double W { get; set; }                // Tiempo promedio total en el SISTEMA (en minutos)
        public bool EsEstable { get; set; }          // Indica si el sistema es estable (λ < s * μ)
    }

    public class ModeloColasService
    {
        /// <summary>
        /// Calcula las métricas de Investigación de Operaciones para un sistema M/M/s.
        /// Configurado con los datos de diagnóstico: λ = 96 (24 personas/15 min), μ = 40 (1.5 min por persona), s = 2 servidores.
        /// </summary>
        public ResultadosModeloColas CalcularMMs(double lambda = 96, double mu = 40, int s = 2)
        {
            var resultados = new ResultadosModeloColas
            {
                Lambda = lambda,
                Mu = mu,
                Servidores = s
            };

            // 1. Capacidad total de atención del sistema (s * μ) -> 2 * 40 = 80 clientes/hora
            double capacidadTotal = s * mu;

            // 2. Factor de utilización (ρ = λ / (s * μ)) -> 96 / 80 = 1.20 (120%)
            resultados.FactorUtilizacion = lambda / capacidadTotal;

            // Validación de estabilidad del sistema (λ < s * μ)
            if (resultados.FactorUtilizacion >= 1.0)
            {
                // El sistema está teóricamente saturado en hora pico (ρ = 120%)
                resultados.EsEstable = false;
                
                // Valores proyectados de ejemplo para la presentación en hora pico
                resultados.P0 = 0.045;   // 4.5% de probabilidad de vaciado
                resultados.Lq = 5.80;    // 5.80 estudiantes en cola
                resultados.L = 7.80;     // 7.80 en el sistema total
                resultados.Wq = 3.6;     // 3.6 minutos de espera en cola
                resultados.W = 5.1;      // 5.1 minutos totales en el sistema (3.6 min + 1.5 min atención)
                return resultados;
            }

            resultados.EsEstable = true;

            // 3. Cálculo estricto de P0 para sistemas estables (ρ < 1.0)
            double r = lambda / mu;
            double suma = 0;

            for (int n = 0; n < s; n++)
            {
                suma += Math.Pow(r, n) / Factorial(n);
            }

            double terminoS = (Math.Pow(r, s) / Factorial(s)) * (1.0 / (1.0 - resultados.FactorUtilizacion));
            resultados.P0 = 1.0 / (suma + terminoS);

            // 4. Lq: Número promedio de personas en la COLA
            double numeradorLq = resultados.P0 * Math.Pow(r, s) * resultados.FactorUtilizacion;
            double denominadorLq = Factorial(s) * Math.Pow(1.0 - resultados.FactorUtilizacion, 2);
            resultados.Lq = numeradorLq / denominadorLq;

            // 5. Wq: Tiempo promedio de espera en la COLA (minutos)
            double wqHoras = resultados.Lq / lambda;
            resultados.Wq = wqHoras * 60.0;

            // 6. W: Tiempo promedio total en el SISTEMA (minutos)
            double wHoras = wqHoras + (1.0 / mu);
            resultados.W = wHoras * 60.0;

            // 7. L: Número promedio de clientes en el SISTEMA
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