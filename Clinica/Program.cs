using System;
using System.Collections.Generic;
using System.Linq;

public class SistemaGestionPacientes
{
    private List<Paciente> pacientes = new List<Paciente>();
    private List<Medicamento> catalogoMedicamentos = new List<Medicamento>();
    private List<Tratamiento> tratamientos = new List<Tratamiento>();


    // se agrega las funciones para el menu principal de la consola
    public void MenuPrincipal()
    {
        int opcion;
        do
        {
            Console.WriteLine("\nMenú Principal");
            Console.WriteLine("1- Agregar paciente");
            Console.WriteLine("2- Agregar medicamento al catálogo");
            Console.WriteLine("3- Asignar tratamiento médico a un paciente");
            Console.WriteLine("4- Consultas");
            Console.WriteLine("5- Salir");
            Console.Write("Seleccione una opción: ");
            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción no válida. Intente nuevamente.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    AgregarPaciente();
                    break;
                case 2:
                    AgregarMedicamento();
                    break;
                case 3:
                    AsignarTratamiento();
                    break;
                case 4:
                    Consultas();
                    break;
                case 5:
                    Console.WriteLine("Saliendo del sistema...");
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        } while (opcion != 5);
    }
    //se empieza a estructurar cada una de la funcion de manera privada y void
    private void AgregarPaciente()
    {
        Console.WriteLine("\nAgregar Paciente");
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Número de cédula: ");
        string cedula = Console.ReadLine();
        if (pacientes.Any(p => p.NumeroCedula == cedula))
        {
            Console.WriteLine("Usuario existente.");
            return;
        }
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Tipo de sangre: ");
        string tipoSangre = Console.ReadLine();
        Console.Write("Dirección: ");
        string direccion = Console.ReadLine();
        Console.Write("Fecha de Nacimiento (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime fechaNacimiento))
        {
            Console.WriteLine("Formato de fecha incorrecto. Ingrese la fecha en formato YYYY-MM-DD.");
            return;
        }

        Paciente paciente = new Paciente(nombre, cedula, telefono, tipoSangre, direccion, fechaNacimiento);
        pacientes.Add(paciente);
        Console.WriteLine("Paciente agregado exitosamente.");
    }

    private void AgregarMedicamento()
    {
        Console.WriteLine("\nAgregar Medicamento al Catálogo");
        Console.Write("Código del medicamento: ");
        string codigo = Console.ReadLine();
        if (catalogoMedicamentos.Any(m => m.Codigo == codigo))
        {
            Console.WriteLine($"Medicamento existente en código {codigo}.");
            return;
        }
        Console.Write("Nombre del medicamento: ");
        string nombre = Console.ReadLine();
        Console.Write("Cantidad: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad))
        {
            Console.WriteLine("Cantidad no válida. Intente nuevamente.");
            return;
        }

        Medicamento medicamento = new Medicamento(codigo, nombre, cantidad);
        catalogoMedicamentos.Add(medicamento);
        Console.WriteLine("Medicamento agregado exitosamente.");
    }

    private void AsignarTratamiento()
    {
        Console.WriteLine("\nAsignar Tratamiento Médico a un Paciente");
        if (pacientes.Count == 0)
        {
            Console.WriteLine("No hay pacientes registrados. Registre un paciente primero.");
            return;
        }
        Console.WriteLine("Seleccione un paciente:");
        for (int i = 0; i < pacientes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {pacientes[i].Nombre} ({pacientes[i].NumeroCedula})");
        }
        if (!int.TryParse(Console.ReadLine(), out int indicePaciente) || indicePaciente < 1 || indicePaciente > pacientes.Count)
        {
            Console.WriteLine("Opción no válida. Intente nuevamente.");
            return;
        }
        Paciente pacienteSeleccionado = pacientes[indicePaciente - 1];
        if (tratamientos.Count(t => t.Paciente == pacienteSeleccionado) >= 3)
        {
            Console.WriteLine("Este paciente ya tiene asignados 3 tratamientos.");
            return;
        }

        Console.WriteLine("Seleccione medicamentos a asignar (ingrese números separados por coma):");
        for (int i = 0; i < catalogoMedicamentos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {catalogoMedicamentos[i].Nombre} ({catalogoMedicamentos[i].Cantidad} disponibles)");
        }
        string[] medicamentosSeleccionados = Console.ReadLine().Split(',');
        List<Medicamento> medicamentosAsignados = new List<Medicamento>();
        foreach (var indiceMedicamento in medicamentosSeleccionados)
        {
            if (!int.TryParse(indiceMedicamento, out int indice) || indice < 1 || indice > catalogoMedicamentos.Count)
            {
                Console.WriteLine("Opción no válida. Intente nuevamente.");
                return;
            }
            var medicamento = catalogoMedicamentos[indice - 1];
            Console.Write($"Ingrese la cantidad de {medicamento.Nombre} a asignar: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad))
            {
                Console.WriteLine("Cantidad no válida. Intente nuevamente.");
                return;
            }
            if (cantidad > medicamento.Cantidad)
            {
                Console.WriteLine($"Lastimosamente no le ofrecemos esa cantidad de {medicamento.Nombre}");
                continue;
            }
            medicamentosAsignados.Add(new Medicamento(medicamento.Codigo, medicamento.Nombre, cantidad));

            // con esta funcion lo que haremos sera actualizar el inventario cada vez que se asigna

            medicamento.Cantidad -= cantidad;
        }
        tratamientos.Add(new Tratamiento(pacienteSeleccionado, medicamentosAsignados));
        Console.WriteLine("Tratamiento asignado exitosamente.");
    }
    private int CalcularEdad(DateTime fechaNacimiento)
    {
        DateTime fechaActual = DateTime.Now;
        int edad = fechaActual.Year - fechaNacimiento.Year;
        if (fechaNacimiento > fechaActual.AddYears(-edad))
        {
            edad--;
        }
        return edad;
    }
    private void Consultas()
    {

        // se separan los reportes para que el usuario escoja y obtenga el reporte deseado 

        Console.WriteLine("\nConsultas");
        Console.WriteLine("Seleccione el tipo de reporte que desea generar:");
        Console.WriteLine("1. Cantidad total de pacientes registrados");
        Console.WriteLine("2. Reporte de todos los medicamentos recetados sin repetirlos");
        Console.WriteLine("3. Reporte de cantidad de pacientes agrupados por edades");
        Console.WriteLine("4. Reporte Pacientes y consultas ordenado por nombre");

        if (!int.TryParse(Console.ReadLine(), out int opcion) || opcion < 1 || opcion > 4)
        {
            Console.WriteLine("Opción no válida. Intente nuevamente.");
            return;
        }

        switch (opcion)
        {
            case 1:
                GenerarReporteCantidadPacientes();
                break;
            case 2:
                GenerarReporteMedicamentosRecetados();
                break;
            case 3:
                GenerarReportePacientesPorEdades();
                break;
            case 4:
                GenerarReportePacientesYConsultasOrdenado();
                break;
        }
    }

    private void GenerarReporteCantidadPacientes()
    {
        Console.WriteLine($"Cantidad total de pacientes registrados: {pacientes.Count}");
    }

    private void GenerarReporteMedicamentosRecetados()
    {
        Console.WriteLine("Reporte de todos los medicamentos recetados sin repetirlos:");
        var medicamentosUnicos = tratamientos.SelectMany(t => t.Medicamentos).Distinct();
        foreach (var medicamento in medicamentosUnicos)
        {
            Console.WriteLine($"{medicamento.Nombre}");
        }
    }

    private void GenerarReportePacientesPorEdades()
    {
        Console.WriteLine("Reporte de cantidad de pacientes agrupados por edades:");
        var pacientesPorEdades = pacientes.GroupBy(p => CalcularEdad(p.FechaNacimiento));
        foreach (var grupo in pacientesPorEdades)
        {
            string rangoEdades = grupo.Key switch
            {
                var edad when edad <= 10 => "0-10 años",
                var edad when edad <= 30 => "11-30 años",
                var edad when edad <= 50 => "31-50 años",
                _ => "mayores de 51 años"
            };
            Console.WriteLine($"{rangoEdades}: {grupo.Count()} pacientes");
        }
    }

    private void GenerarReportePacientesYConsultasOrdenado()
    {
        Console.WriteLine("Reporte Pacientes y consultas ordenado por nombre:");
        var pacientesOrdenados = pacientes.OrderBy(p => p.Nombre);
        foreach (var paciente in pacientesOrdenados)
        {
            Console.WriteLine($"Paciente: {paciente.Nombre} ({paciente.NumeroCedula})");
            var tratamientosPaciente = tratamientos.Where(t => t.Paciente == paciente);
            Console.WriteLine($"Tratamientos:");
            foreach (var tratamiento in tratamientosPaciente)
            {
                Console.WriteLine($"- Medicamentos: {string.Join(", ", tratamiento.Medicamentos.Select(m => $"{m.Nombre} ({m.Cantidad})"))}");
            }
        }
    }

    public class Paciente

        // se aplica los metodos get / set
    {
        public string Nombre { get; }
        public string NumeroCedula { get; }
        public string Telefono { get; }
        public string TipoSangre { get; }
        public string Direccion { get; }
        public DateTime FechaNacimiento { get; }

        public Paciente(string nombre, string numeroCedula, string telefono, string tipoSangre, string direccion, DateTime fechaNacimiento)
        {
            Nombre = nombre;
            NumeroCedula = numeroCedula;
            Telefono = telefono;
            TipoSangre = tipoSangre;
            Direccion = direccion;
            FechaNacimiento = fechaNacimiento;
        }
    }

    public class Medicamento
    {
        public string Codigo { get; }
        public string Nombre { get; }
        public int Cantidad { get; set; }

        public Medicamento(string codigo, string nombre, int cantidad)
        {
            Codigo = codigo;
            Nombre = nombre;
            Cantidad = cantidad;
        }
    }

    public class Tratamiento
    {
        public Paciente Paciente { get; }
        public List<Medicamento> Medicamentos { get; }

        public Tratamiento(Paciente paciente, List<Medicamento> medicamentos)
        {
            Paciente = paciente;
            Medicamentos = medicamentos;
        }
        // y por ultimo el main para ejecutar el programa

        public static void Main(string[] args)

        {
            SistemaGestionPacientes sistema = new SistemaGestionPacientes();
            sistema.MenuPrincipal();
        }
    }
}