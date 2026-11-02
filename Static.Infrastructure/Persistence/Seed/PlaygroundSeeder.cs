using Microsoft.EntityFrameworkCore;
using Static.Domain.Entities.Playground;
using Static.Domain.Enums;
using Static.Infrastructure.Persistence;

namespace Static.Infrastructure.Persistence.Seed;

public sealed class PlaygroundSeeder(StaticDbContext db) : ISeeder
{
    public async Task SeedAsync()
    {
        if (!await db.Lesson.AnyAsync())
        {
            var lessons = BuildLessons();
            await db.Lesson.AddRangeAsync(lessons);
            await db.SaveChangesAsync();
        }

        // Concept / Visualize blocks, walkthroughs, challenges and quizzes for the lessons above.
        await PlaygroundLessonContentSeed.ApplyAsync(db);
    }

    // ── Lessons ───────────────────────────────────────────────────────────────

    private static List<Lesson> BuildLessons() =>
    [
        // ── OOP ──────────────────────────────────────────────────────────────
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Classes and Objects",
            Description = "A class is a blueprint; an object is an instance. Learn how to define attributes and methods.",
            Category = LessonCategory.OOP,
            OrderIndex = 1,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Basic Class",
                    Description = "Define a Dog class with a name attribute and a bark method.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    ExpectedOutput = "Buddy says: Woof!",
                    Code = """
class Dog:
    def __init__(self, name):
        self.name = name

    def bark(self):
        print(f"{self.name} says: Woof!")

dog = Dog("Buddy")
dog.bark()
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Encapsulation",
            Description = "Bundle data and methods together and restrict direct access using private attributes.",
            Category = LessonCategory.OOP,
            OrderIndex = 2,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Private Attributes",
                    Description = "Use name mangling (__attr) to hide internal state and expose it via getters/setters.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    ExpectedOutput = "Balance: 1000\nBalance after deposit: 1500\nInsufficient funds.",
                    Code = """
class BankAccount:
    def __init__(self, balance):
        self.__balance = balance   # private

    def deposit(self, amount):
        if amount > 0:
            self.__balance += amount

    def withdraw(self, amount):
        if amount > self.__balance:
            print("Insufficient funds.")
        else:
            self.__balance -= amount

    def get_balance(self):
        return self.__balance

account = BankAccount(1000)
print("Balance:", account.get_balance())
account.deposit(500)
print("Balance after deposit:", account.get_balance())
account.withdraw(2000)
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Inheritance",
            Description = "A child class inherits attributes and methods from a parent class, enabling code reuse.",
            Category = LessonCategory.OOP,
            OrderIndex = 3,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Animal Hierarchy",
                    Description = "Create Animal as a base class and Dog / Cat as subclasses.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    ExpectedOutput = "Buddy says Woof\nWhiskers says Meow",
                    Code = """
class Animal:
    def __init__(self, name):
        self.name = name

    def speak(self):
        raise NotImplementedError

class Dog(Animal):
    def speak(self):
        return f"{self.name} says Woof"

class Cat(Animal):
    def speak(self):
        return f"{self.name} says Meow"

for animal in [Dog("Buddy"), Cat("Whiskers")]:
    print(animal.speak())
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Polymorphism",
            Description = "The same method name behaves differently depending on the object's type.",
            Category = LessonCategory.OOP,
            OrderIndex = 4,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Shape Area",
                    Description = "Calculate area for Circle and Rectangle through a common interface.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    ExpectedOutput = "Circle area: 78.54\nRectangle area: 24",
                    Code = """
import math

class Shape:
    def area(self):
        raise NotImplementedError

class Circle(Shape):
    def __init__(self, radius):
        self.radius = radius

    def area(self):
        return round(math.pi * self.radius ** 2, 2)

class Rectangle(Shape):
    def __init__(self, width, height):
        self.width = width
        self.height = height

    def area(self):
        return self.width * self.height

shapes = [Circle(5), Rectangle(4, 6)]
for shape in shapes:
    print(f"{type(shape).__name__} area: {shape.area()}")
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Abstraction",
            Description = "Hide complex implementation details behind a simple interface using abstract classes.",
            Category = LessonCategory.OOP,
            OrderIndex = 5,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Abstract Payment",
                    Description = "Use ABC to define a Payment interface that concrete classes must implement.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
from abc import ABC, abstractmethod

class Payment(ABC):
    @abstractmethod
    def pay(self, amount):
        pass

class CreditCard(Payment):
    def pay(self, amount):
        print(f"Paid ${amount} via Credit Card.")

class PayPal(Payment):
    def pay(self, amount):
        print(f"Paid ${amount} via PayPal.")

for gateway in [CreditCard(), PayPal()]:
    gateway.pay(100)
"""
                }
            ]
        },

        // ── SOLID ─────────────────────────────────────────────────────────────
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Single Responsibility Principle",
            Description = "A class should have one, and only one, reason to change.",
            Category = LessonCategory.SOLID,
            OrderIndex = 1,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Separate Report and Printer",
                    Description = "Split report logic from printing logic so each class has a single job.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
# ❌ Bad: one class does two things
class ReportBad:
    def generate(self):
        return "Sales Report Data"

    def print_report(self):
        print(self.generate())   # mixing concerns

# ✅ Good: split into two focused classes
class Report:
    def generate(self):
        return "Sales Report Data"

class Printer:
    def print_report(self, report: Report):
        print(report.generate())

Printer().print_report(Report())
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Open/Closed Principle",
            Description = "Classes should be open for extension but closed for modification.",
            Category = LessonCategory.SOLID,
            OrderIndex = 2,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Discount Strategy",
                    Description = "Add new discount types by creating new classes, not editing existing ones.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
from abc import ABC, abstractmethod

class Discount(ABC):
    @abstractmethod
    def apply(self, price):
        pass

class NoDiscount(Discount):
    def apply(self, price):
        return price

class TenPercent(Discount):
    def apply(self, price):
        return price * 0.9

class BlackFriday(Discount):
    def apply(self, price):
        return price * 0.5

def checkout(price, discount: Discount):
    final = discount.apply(price)
    print(f"Final price: ${final:.2f}")

checkout(100, NoDiscount())
checkout(100, TenPercent())
checkout(100, BlackFriday())
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Liskov Substitution Principle",
            Description = "Subclasses must be replaceable for their base class without breaking the program.",
            Category = LessonCategory.SOLID,
            OrderIndex = 3,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Bird Flight Example",
                    Description = "Avoid forcing Penguin to implement fly() by redesigning the hierarchy.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
# ❌ Bad: Penguin can't fly but is forced to implement fly()
class BirdBad:
    def fly(self):
        pass

class PenguinBad(BirdBad):
    def fly(self):
        raise Exception("Penguins can't fly!")

# ✅ Good: separate flying ability into a mixin
class Bird:
    def eat(self):
        print(f"{type(self).__name__} is eating.")

class FlyingBird(Bird):
    def fly(self):
        print(f"{type(self).__name__} is flying.")

class Eagle(FlyingBird):
    pass

class Penguin(Bird):
    pass

Eagle().fly()
Eagle().eat()
Penguin().eat()
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Interface Segregation Principle",
            Description = "No class should be forced to implement methods it does not use.",
            Category = LessonCategory.SOLID,
            OrderIndex = 4,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Worker Interfaces",
                    Description = "Split a fat Worker interface into Workable and Eatable so robots aren't forced to eat.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
from abc import ABC, abstractmethod

# ✅ Small, focused interfaces
class Workable(ABC):
    @abstractmethod
    def work(self): pass

class Eatable(ABC):
    @abstractmethod
    def eat(self): pass

class Human(Workable, Eatable):
    def work(self):
        print("Human working.")
    def eat(self):
        print("Human eating.")

class Robot(Workable):
    def work(self):
        print("Robot working.")

Human().work(); Human().eat()
Robot().work()
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Dependency Inversion Principle",
            Description = "High-level modules should not depend on low-level modules; both should depend on abstractions.",
            Category = LessonCategory.SOLID,
            OrderIndex = 5,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Notification Service",
                    Description = "Inject a Notifier abstraction so the Order class works with Email, SMS, or Push.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
from abc import ABC, abstractmethod

class Notifier(ABC):
    @abstractmethod
    def send(self, message): pass

class EmailNotifier(Notifier):
    def send(self, message):
        print(f"Email: {message}")

class SMSNotifier(Notifier):
    def send(self, message):
        print(f"SMS: {message}")

class Order:
    def __init__(self, notifier: Notifier):
        self.notifier = notifier

    def place(self, item):
        print(f"Order placed for {item}.")
        self.notifier.send(f"Your order for {item} is confirmed!")

Order(EmailNotifier()).place("Laptop")
Order(SMSNotifier()).place("Phone")
"""
                }
            ]
        },

        // ── Design Patterns ───────────────────────────────────────────────────
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Singleton Pattern",
            Description = "Ensure a class has only one instance and provide a global access point to it.",
            Category = LessonCategory.DesignPatterns,
            OrderIndex = 1,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Config Manager",
                    Description = "Use __new__ to guarantee only one Config instance exists throughout the program.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
class Config:
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance.debug = False
        return cls._instance

c1 = Config()
c2 = Config()
c1.debug = True

print("Same instance?", c1 is c2)   # True
print("c2.debug:", c2.debug)         # True — same object
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Factory Pattern",
            Description = "Define an interface for creating objects, but let subclasses decide which class to instantiate.",
            Category = LessonCategory.DesignPatterns,
            OrderIndex = 2,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Vehicle Factory",
                    Description = "Use a factory function to create Car or Bike objects without exposing constructors.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
class Car:
    def drive(self):
        print("Driving a car.")

class Bike:
    def drive(self):
        print("Riding a bike.")

class VehicleFactory:
    @staticmethod
    def create(vehicle_type: str):
        vehicles = {"car": Car, "bike": Bike}
        cls = vehicles.get(vehicle_type.lower())
        if cls is None:
            raise ValueError(f"Unknown vehicle: {vehicle_type}")
        return cls()

VehicleFactory.create("car").drive()
VehicleFactory.create("bike").drive()
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Observer Pattern",
            Description = "Define a one-to-many dependency so that when one object changes state, all its dependents are notified.",
            Category = LessonCategory.DesignPatterns,
            OrderIndex = 3,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Event System",
                    Description = "Build a simple event bus where subscribers react to publisher state changes.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
class EventBus:
    def __init__(self):
        self._subscribers = []

    def subscribe(self, fn):
        self._subscribers.append(fn)

    def publish(self, event):
        for fn in self._subscribers:
            fn(event)

bus = EventBus()

bus.subscribe(lambda e: print(f"Logger received: {e}"))
bus.subscribe(lambda e: print(f"EmailService received: {e}"))

bus.publish("user.registered")
bus.publish("order.placed")
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Strategy Pattern",
            Description = "Define a family of algorithms, encapsulate each one, and make them interchangeable at runtime.",
            Category = LessonCategory.DesignPatterns,
            OrderIndex = 4,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Sorting Strategy",
                    Description = "Switch between Bubble Sort and Quick Sort without changing the Sorter class.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
from abc import ABC, abstractmethod

class SortStrategy(ABC):
    @abstractmethod
    def sort(self, data): pass

class BubbleSort(SortStrategy):
    def sort(self, data):
        d = data[:]
        for i in range(len(d)):
            for j in range(len(d) - i - 1):
                if d[j] > d[j+1]:
                    d[j], d[j+1] = d[j+1], d[j]
        return d

class BuiltinSort(SortStrategy):
    def sort(self, data):
        return sorted(data)

class Sorter:
    def __init__(self, strategy: SortStrategy):
        self.strategy = strategy

    def sort(self, data):
        return self.strategy.sort(data)

data = [5, 2, 8, 1, 9]
print(Sorter(BubbleSort()).sort(data))
print(Sorter(BuiltinSort()).sort(data))
"""
                }
            ]
        },
        new Lesson
        {
            Id = Guid.NewGuid(),
            Title = "Decorator Pattern",
            Description = "Attach additional responsibilities to an object dynamically without modifying its class.",
            Category = LessonCategory.DesignPatterns,
            OrderIndex = 5,
            IsPublished = true,
            CodeExamples =
            [
                new CodeExample
                {
                    Id = Guid.NewGuid(),
                    Title = "Coffee Decorator",
                    Description = "Add Milk and Sugar to a BasicCoffee by wrapping it with decorator classes.",
                    Language = "python",
                    OrderIndex = 1,
                    IsEditable = true,
                    Code = """
class Coffee:
    def cost(self): return 5
    def description(self): return "Basic Coffee"

class MilkDecorator:
    def __init__(self, coffee):
        self._coffee = coffee
    def cost(self): return self._coffee.cost() + 2
    def description(self): return self._coffee.description() + ", Milk"

class SugarDecorator:
    def __init__(self, coffee):
        self._coffee = coffee
    def cost(self): return self._coffee.cost() + 1
    def description(self): return self._coffee.description() + ", Sugar"

order = SugarDecorator(MilkDecorator(Coffee()))
print(order.description())
print(f"Total: ${order.cost()}")
"""
                }
            ]
        }
    ];
}
