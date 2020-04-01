using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global, UnusedAutoPropertyAccessor.Global
namespace LabTest.Models
{
    public static class SimpleMock
    {
        public static readonly List<Employee> Employees = new List<Employee>();

        static SimpleMock()
        {
            for (var i = 0; i < 10; i++)
                Employees.Add(new Employee
                {
                    Id = i + 1, Name = $"Name{i + 1}", Surname = $"Surname{i + 1}",
                    Position = (Position) new Random().Next(0, Enum.GetNames(typeof(Position)).Length),
                    DummyColumn1 = $"DummyColumn{i + 1}", DummyColumn2 = $"DummyColumn{i + 1}"
                });
        }
    }

    public class Employee
    {
        public int Id { get; set; }

        [Required, MinLength(5), MaxLength(15)]
        public string Name { get; set; }

        [Required, MinLength(5), MaxLength(15)]
        public string Surname { get; set; }

        [Required] public Position Position { get; set; }
        public string DummyColumn1 { get; set; }
        public string DummyColumn2 { get; set; }
    }

    public enum Position
    {
        Architect,
        TeamLead,
        Senior,
        Middle,
        Junior,
        Trainee
    }

    public class SimpleViewModel
    {
        public readonly List<SimpleViewItem> ViewEmployees;
        public SimpleViewModel(List<SimpleViewItem> viewEmployees) => ViewEmployees = viewEmployees;
    }

    public class SimpleViewItem
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Surname;
        public readonly string Position;

        public SimpleViewItem(int id, string name, string surname, string position)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Position = position;
        }
    }
}