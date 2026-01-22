# 🏦 C# Banking System Simulation

A console-based banking application built to demonstrate the core principles of Object-Oriented Programming (OOP) in C#.

## 🚀 Overview
This project simulates a banking engine that handles different types of accounts with unique behaviors. It processes transactions, calculates interest, and manages month-end reporting using a polymorphic list.

## 🔑 Key Features
* **Standard Savings Account:** Earns interest at the end of the month.
* **Gift Card Account:** One-time use; balance resets to 0 at the end of the month.
* **Line of Credit:** Allows negative balances (debt) and charges a monthly fee on the owed amount.
* **Transaction History:** Automatically tracks every deposit and withdrawal with timestamps.

## 🧠 OOP Concepts Applied
1.  **Encapsulation:** Protected `Balance` property ensures data cannot be modified directly.
2.  **Inheritance:** Shared logic (ID, Name, History) is inherited from the base `BankAccount` class.
3.  **Polymorphism:** `PerformMonthEndTransactions()` behaves differently for each account type.
4.  **Abstraction:** The `BankAccount` class prevents the creation of generic "ghost" accounts.

## 🛠️ Built With
* C#
* .NET
* Git & GitHub