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
    

## About The Project
We implement the algorithms to get residual sensitivity and elastic sensitivity for multi-way join query and we demo our implementations on 8 experimental queries shown in our paper. Here, two datasets: TPC-H dataset and Facebook ego-network dataset are provided. The algorithm to calculate residual sensitivity/elastic sensitivity contains two parts: collecting required TE's(mf's) and computing residual sensitivity/elstic sensitivity based on the collected information. Here we provide codes to realize three main functions:

+ Import/Export data to/from database;
+ Demo the algorithm to compute residual sensitivity/elastic sensitivity given one query;
+ Demo how to collect experimental results shown in our paper;

## Prerequisites
### Tools
Before running this project, please install below tools

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
Create an empty postgresql database. For example, create a database named as `test`:
```sh
createdb test
```

## Demo the Project
### Import Data
The data are store in `./data`. The TPC-H datasets are stored in `./data/TPCH`, where we provide 7 datasets with different scales: `0.01, 0.05, 0.1, 0.5, 1, 5, 10`. As space limitation, we only upload the dataset with scale `0.01, 0.05, 0.1`. The others are avaliable [here](https://drive.google.com/file/d/1DdFp7jQK1gt8VLUhrv0H4YrvpjcDCiDI/view?usp=sharing). The TPC-H data are used by query Q1 to Q3. The Facebook ego-network dataset is stored in `./data/Facebook` and is used by query Q4 to Q8.

To import data into postgresql database, change the working directory to `./code` and run `ImportDataToDB.py`, which has three arguments: `-D` indicates the database name, `-d` indicates dataset name, `-s` indicates the dataset scale. Use `-h` to check the details. For example, use below commands to import TPC-H dataset with scale 0.01 into database `test`.

```sh
cd ./code
python ImportDataToDB.py -s 0.01 -D test -d TPCH
```

### Export Data
To export dataset from postgresql database, change the working directory to `./code` and run `ExportDataFromDB.py`, which has two arguments: `-D` indicates the database name, `-d` indicates dataset name. For example, use below commands to export TPC-H dataset from database `test`.
```sh
cd ./code
python ExportDataFromDB.py -D test -d TPCH
```

## Compute Residual Sensitivity
The algorithm of residual senstivity can be divided into two parts: collect TE's and compute residual senstivity. As mentioned in paper, for the first part, given one query, we need to manually write the queries and run them to get TE's. For convenience and simplicity, we store the queries required in files under directory `./query` and use a python program to run these queries in postgresql server. The collected TE's are stored in files under directory `./temp`. For the second part, we implement a C++ program. Since we can use C language to write User-Define-Function in postgresql, this can be regarded as one UDF of postgresql. Here, for convenience, we also provide a python program to help call the C++ program.

* To collect TE's, change the working directory to `./code` and run `CollectTE.py`, which has three arguments: `-D` indicates the database, `-Q` indicates the query ID, `-s` indicates the scale(only required for TPC-H queries). Use `-h` to check the details. For example, run below commands if we want to collect TE's for Q1 of scale 0.01 in test database.
```sh
cd ./code
python CollectTE.py -D test -s 0.01 -Q 1
```
* To compute residual sensitivity, run `ComputeRS.py`, which has three arguments: `-Q` indicates the query ID, `-s` indicates the scale(only required for TPC-H queries), `-B` indicates the value of $\beta$. For example, run below command if we want to compute residual sensitivity for Q1 of scale 0.01 dataset with $\beta=1$.
```sh
python ComputeRS.py -s 0.01 -Q 1 -B 0.01
```

## Compute Elastic Sensitivity
The algorithm for elastic sensitivity also contains two steps: collecting mf's and computing elastic sensitivity.

* To collect mf's, change the working directory to `./code` and run `Collectmf.py`, which has same arguments with `CollectTE.py`. For example, run below commands if we want to collect mf's for Q1 of scale 0.01 in test database.
```sh
cd ./code
python Collectmf.py -D test -s 0.01 -Q 1
```
* To compute residual sensitivity, run `ComputeES.py`, which has same arguments with `ComputeRS.py`. For example, run below command if we want to compute elastic sensitivity for Q1 of scale 0.01 dataset with $\beta=1$.
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
