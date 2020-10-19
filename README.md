# Demo of Residual Sensitivity
## Table of Contents
* [About the Project](#about-the-project)  
* [Prerequisites](#prerequisites)
    * [Tools](#tools)
    * [Python Dependency](#python-dependency)
    * [Create an Empty Database](#create-an-empty-database)
* [Demo the Project](#demo-the-project)
    * [Import Data](#import-data)
    * [Export Data](#export-data)
    * [Compute Residual Sensitivity](#compute-residual-sensitivity)
    * [Compute Elastic Sensitivity](#compute-elastic-sensitivity)
    * [Collect Statistic for Table 1](#collect-statistic-for-table-1)
    * [Draw Figure 5](#draw-figure-5)
    * [Draw Figure 6](#draw-figure-6)
* [Demo System](#demo-system)
* [Demo the Tests with wPINQ](#demo-the-tests-with-wpinq)

## About The Project
First, we implement the algorithms to get residual sensitivity and elastic sensitivity for multi-way join queries and we demo our implementations on 8 experimental queries as shown in our paper. Here, two datasets: the TPC-H dataset and the Facebook ego-network dataset are used. We provide codes to realize three main functions:

+ Import/Export data to/from database;
+ Demo the algorithm to compute residual sensitivity/elastic sensitivity given one query;
+ Demo how to collect experimental results shown in our paper;

Second, we implement a complete system for residual sensitivity. Third, we test wPINQ with our experimental data.


## Prerequisites
### Tools
Before running this project, please install the below tools

* [PostgreSQL](https://www.postgresql.org/)
* [Python3](https://www.python.org/download/releases/3.0/)
* [Cmake](https://cmake.org/)

### Python Dependency
Here are dependencies used in python codes:
* `matplotlib`
* `numpy`
* `sys`
* `getopt`
* `os`
* `math`
* `psycopg2`
### Compile C++ code
Before run the demo, please compile the C++ code. 
```sh
cd ./code/RSESCalculator/
cmake .
make
```
### Create an Empty Database
Create an empty PostgreSQL database. For example, create a database named `test`:
```sh
createdb test
```

## Demo the Project
### Import Data
The data are store in `./data`. The TPC-H datasets are stored in `./data/TPCH`, where we provide 7 datasets with different scales: `0.01, 0.05, 0.1, 0.5, 1, 5, 10`. As space limitation, we only upload the dataset with scale `0.01, 0.05, 0.1`. The others are available [here](https://drive.google.com/file/d/1DdFp7jQK1gt8VLUhrv0H4YrvpjcDCiDI/view?usp=sharing). The TPC-H data are used by query Q1 to Q3. The Facebook ego-network dataset is stored in `./data/Facebook` and is used by query Q4 to Q8.

To import data into the PostgreSQL database, change the working directory to `./code` and run `ImportDataToDB.py`, which has three arguments: `-D` indicates the database name, `-d` indicates dataset name, `-s` indicates the dataset scale. Use `-h` to check the details. For example, use the below commands to import the TPC-H dataset with scale 0.01 into the database `test`.

```sh
cd ./code
python ImportDataToDB.py -s 0.01 -D test -d TPCH
```

### Export Data
To export data from the PostgreSQL database, change the working directory to `./code` and run `ExportDataFromDB.py`, which has two arguments: `-D` indicates the database name, `-d` indicates dataset name. For example, use the below commands to export the TPC-H dataset from the database `test`.
```sh
cd ./code
python ExportDataFromDB.py -D test -d TPCH
```

## Compute Residual Sensitivity
The algorithm of residual sensitivity can be divided into two parts: collect TE's and compute residual sensitivity. As mentioned in the paper, for the first part, given one query, we need to manually write the queries and run them to get TE's. For convenience and simplicity, we store the queries required in files under directory `./query` and use a python program to run these queries in the PostgreSQL server. The collected TE's are stored in files under directory `./temp`. For the second part, we implement a C++ program. Since we can use C language to write User-Define-Function in PostgreSQL, this can be regarded as one UDF of PostgreSQL. Here, for convenience, we also provide a python program to help call the C++ program.

* To collect TE's, change the working directory to `./code` and run `CollectTE.py`, which has three arguments: `-D` indicates the database, `-Q` indicates the query ID, `-s` indicates the scale(only required for TPC-H queries). Use `-h` to check the details. For example, run the below commands if we want to collect TE's for Q1 of scale 0.01 in the test database.
```sh
cd ./code
python CollectTE.py -D test -s 0.01 -Q 1
```
* To compute residual sensitivity, run `ComputeRS.py`, which has three arguments: `-Q` indicates the query ID, `-s` indicates the scale(only required for TPC-H queries), `-B` indicates the value of $\beta$. For example, run the below command if we want to compute residual sensitivity for Q1 of scale 0.01 dataset with $\beta=1$.
```sh
python ComputeRS.py -s 0.01 -Q 1 -B 0.01
```

## Compute Elastic Sensitivity
The algorithm for elastic sensitivity also contains two steps: collecting mf's and computing elastic sensitivity.

* To collect mf's, change the working directory to `./code` and run `Collectmf.py`, which has the same arguments as `CollectTE.py`. For example, run the below commands if we want to collect mf's for Q1 of scale 0.01 in the test database.
```sh
cd ./code
python Collectmf.py -D test -s 0.01 -Q 1
```
* To compute residual sensitivity, run `ComputeES.py`, which has the same arguments as `ComputeRS.py`. For example, run the below command if we want to compute elastic sensitivity for Q1 of scale 0.01 dataset with $\beta=1$.
```sh
python ComputeES.py -s 0.01 -Q 1 -B 0.01
```

## Collect Statistic for Table 1
The statistic involved in table 1 is divided into two parts: values of residual sensitivity and elastic sensitivity and running time of algorithms of residual sensitivity and elastic sensitivity.

* To collect the statistic related to the value, change the working directory to `./code` and run the `CollectTE.py`.
```sh
cd ./code
python CompareRSWithES.py
```

* To collect the statistic related to running time, change the working directory to `./code` and run the `CompareRunningTime.py`.
```sh
cd ./code
python CompareRunningTime.py
```

## Draw Figure 5
To draw figure 5, change the working directory to `./code` and run `DrawFig5Graphs.py`.
```sh
cd ./code
python DrawFig5Graphs.py
```

## Draw Figure 6
To draw figure 6, change the working directory to `./code` and run `DrawFig6Graphs.py`.
```sh
cd ./code
python DrawFig6Graphs.py
```

# Demo System
Here, we implement a demo system integrated with PostgreSQL. Currently, the system supports multi-way counting queries with projection, predicate and group-by but without self-joins. The inputs here are a file containing a counting query with the below format
```sh
SELECT COUNT([DISTINCT] *|column_name) FROM table_name 
WHERE conditions 
[GROUP BY grouping_element];
```
a file including the list of private relations and a set of private parameters like the privacy budget. The output is a noised query result(a noised histogram). The demo-system is stored in `./demosystem`.

Before running the system, please import the data into a PostgreSQL database. You can import TPC-H/Facebook data as our previous instructions. Then, please compile the C++ code.
```
cd ./demosystem/RSCalculator
cmake .
make
```
Then, back to `./demosystem` and test the system. There are six parameters

 - `-Q`: the path of input query file. Here, we provide 8 experimental queries in `./demosystem/demoquery`.
 - `-P`: the path of the file containing the list of private relations. Here, we provide 8 experimental queries in `./demosystem/demoquery`.
 - `-e`: privacy budget $\epsilon$.
 - `-d`: privacy budget $\delta$.
 - `-N`: noise mechanism: `0` for Laplace noise, `1` for Cauchy noise.
 - `-D`: the name of the PostgreSQL database. 

For example, we run the system with `Q5`
```
python RunDemoSystem.py -Q ./demoquery/Q5.txt -P ./demoquery/Q5_private_relations.txt -e 1 -d 0.000000001 -N 0 -D test
```
Here, we provide the 8 queries used in our paper as the demo queries. Besides, we also provide `Q1, Q2` with projection, predicate and group-by.

Currently, we only implement a part of optimizations for the boundary query mentioned in our paper thus the running time for `Q3, Q5, Q7` will be a bit long. We will implement all optimizations and support more complex queries(involving self-joins) in the next version.

# Demo the Tests with wPINQ
Here, we also test the algorithm of wPINQ with 8 experimental queries with a similar experimental setting with [previous work](http://www.vldb.org/pvldb/vol11/p526-johnson.pdf). We modify the wPINQ codes to support our experiments. The wPINQ is under `./VLDB2014 Weighted PINQ`.

Before run wPINQ, 
 - Download the testing data [here](https://drive.google.com/file/d/1IGONyHcVCHdK2EUfQ3eZJ46UIv2IBklL/view?usp=sharing). 
 - Unzip the file. Make sure there is a directory named `InputData` now.
 - Modify `./VLDB2014 Weighted PINQ/VLDB2014 Experiments/Program.cs`: in `13th` line, change that to the absolute path of `InputData`.

Then, follow the [previous instructions](http://faculty.marshall.usc.edu/Davide-Proserpio/wPINQ.html) to run wPINQ. We advise using `Visual Studio` to run the project.
