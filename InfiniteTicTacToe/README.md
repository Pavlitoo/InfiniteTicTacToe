# 🎮 Tic-Tac-Toe Ultra (WPF)

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-%231572B6.svg?style=for-the-badge&logo=windows&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

**Tic-Tac-Toe Ultra** — це сучасна реалізація класичної гри «Хрестики-нулики», створена з використанням технології Windows Presentation Foundation (WPF) та мови програмування C#. Проект розроблено в рамках курсової роботи для демонстрації принципів об'єктно-орієнтованого програмування (ООП) та архітектурного патерну MVVM.

## ✨ Основний функціонал

- 🤖 **Гра проти комп'ютера (ШІ):** Евристичний алгоритм, який аналізує дошку, блокує загрози та знаходить переможні комбінації.
- 👥 **Режим двох гравців:** Локальний мультиплеєр за одним комп'ютером.
- 🪙 **Інтерактивне підкидання монетки:** Анімована система випадкового визначення першого ходу в режимі двох гравців.
- 🎨 **Сучасний UI/UX:** Темна тема (Dark Mode), векторна графіка фігур (без втрати якості), кастомні діалогові вікна.
- ⏱️ **Таймер партії** та система валідації введення імен гравців.

## 🏗 Архітектура

Проект побудований за патерном **Model-View-ViewModel (MVVM)**, що забезпечує чітке розділення логіки та інтерфейсу:
- **Model:** Математичне ядро (`GameEngine`), структури даних та логіка ШІ.
- **ViewModel:** Управління станами інтерфейсу, асинхронні операції (`async/await`) та маршрутизація команд (`ICommand`).
- **View:** Декларативний XAML-інтерфейс, прив'язки даних (`Data Binding`), тригери та анімації.


## 🚀 Встановлення та запуск

1. Склонуйте репозиторій на свій комп'ютер:
   ```bash
   git clone [https://github.com/Pavlitoo/InfiniteTicTacToe.git](https://github.com/Pavlitoo/InfiniteTicTacToe.git)

### 👨‍💻 Автор

 - Павло Луговий

 - Студент групи 101-пТК

[GitHub Профіль](https://github.com/Pavlitoo)