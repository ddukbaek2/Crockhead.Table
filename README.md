# Crockhead.Table

## 개요
자체적인 규칙이 있는 XLSX 파일로부터 데이터테이블에 대한 CS 파일과 JSON 파일을 생성하며, 해당 테이블을 불러오는 기능들을 제공합니다.

## 배포
[![NuGet version](https://img.shields.io/nuget/v/Crockhead.Table.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/Crockhead.Table/)
[![NuGet downloads](https://img.shields.io/nuget/dt/Crockhead.Table.svg?style=flat)](https://www.nuget.org/packages/Crockhead.Table/)

Generates C# and JSON files from Custom-Rule-Based XLSX Tables and Provides Functionality to Load These Tables.     

## 설치
~~~bash
dotnet add package Crockhead.Table
~~~
~~~powershell
Install-Package Crockhead.Table
~~~

## 라이브러리 기반
- netstandard2.1  
- Newtonsoft.Json
- ExcelDataReader
- ExcelDataReader.DataSet
- Crockhead.Core
 
## 네임스페이스
~~~csharp
using Crockhead.Table;
~~~

## 데이터테이블의 정의
테이블은 특정 데이터들을 필드 멤버로 두고 있는 커스텀 구조체의 배열입니다.   
구조체의 필드 멤버는 대체적으로 원시타입이며 하나의 데이터테이블은 반드시 하나의 구조체로 정의됩니다.   
이는 프로그램에서 코드와 데이터를 분리하기 위함이며 데이터의 관리는 물론이고 데이터 제공자와 프로그램 개발자의 정해진 규약으로 개발 생산성을 향상 시킵니다.   

## 데이터테이블의 사용
먼저 데이터 제공자는 데이터를 엑셀 등의 작업이 용의한 애플리케이션으로 작성 후 파일로 저장합니다.   
이후 개발자는 프로그램 내에서 해당 파일을 읽어 안에 있는 데이터를 프로그램에 실제 적용합니다.   

## 라이브러리의 의의
개발자는 데이터를 적용하기 위해서 여러가지 작업들을 선행해야 합니다.   
데이터 제공자가 만든 파일을 파일 포맷에 맞게 읽어야 하고, 읽은 파일 안의 데이터들을 규칙에 맞게 정리하여 프로그램 내에서 접근이 용의하도록 인스턴스화 시켜야 합니다.   
그리고 각 데이터의 연관관계 등 특정 규칙을 통해 이를 논리적으로 가져와 프로그램에 적용해야 합니다.   
하나의 프로그램에서 이러한 데이터는 수천, 수만건이 될 수 있고 혹은 이런 데이터가 수십, 수백가지의 종류도 될 수 있습니다.   
테이블 라이브러리를 사용하면 이 과정을 단순하게 만들어주어 해당 작업 비용을 최소한으로 줄일 수 있습니다.   

## 라이브러리를 사용하지 않을 경우
아래 나열하는 과정이 끝나야 비로소 실제로 데이터의 사용이 가능해집니다.
1. 엑셀 파일에서 셀을 읽어와야 하며 이를 바탕으로 JSON 등의 데이터 시트를 만들어야 합니다.
2. 만든 데이터 시트에 알맞은 구조체 코드를 작성해야 합니다.
3. 만든 데이터 시트를 로드해서 전역 접근이 가능한 구조체로 인스턴스화해야 합니다.
4. 새로운 데이터 시트가 추가 될 때마다 이를 반복해야 합니다.

즉, 이 과정에 해당하는 작업 비용을 줄이기 위해서는 라이브러리를 사용해야 합니다.


## XLSX 파일의 데이터테이블 규칙
- 시트이름 중 #이 접두어로 붙은 경우만 데이터테이블로 변환합니다.
- A열은 프로퍼티로 고정되며 프로퍼티는 해당 행의 타입을 정의하는 용도입니다. 이후 하나의 열마다 하나의 필드를 정의 합니다.
- 하나의 데이터테이블에는 반드시 Id 열을 포함해야하며 이는 정수형 타입으로 1건의 데이터테이블에 대한 고유식별자를 정의합니다.
- A열의 프로퍼티는 Field, Type, Record 타입의 문자열을 값으로 가질 수 있으며 비어있을 경우 해당 행은 해석하지 않고 스킵합니다. (대소문자는 가리지 않습니다.)


