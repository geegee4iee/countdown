# Countdown Application

An application that helps users to monitor their actively using apps.

## Setup
- IDE: Visual Studio 2015, 2017
- .NET Framework: 4.6.1, 2.1RC1
- Database: 
    - **MongoDB** (Should be configured with default port 27017, I haven't set up to allow configurable port yet)

## Included VS Main Projects
- Countdown.Spa.ReactJS: Single-Page-Application using ReactJS and Redux template which is integrated with ASP.Net Core. Purpose: serving statistics and reports to users, also provides some APIs to classify application types via trained Machine Learning model.
- CountdownWPF: Tracking users' active applications.
- Countdown.Core: Provides core entities and interfaces for projects above.