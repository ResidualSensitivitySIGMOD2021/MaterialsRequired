# -*- coding: utf-8 -*-
import sys, getopt
import os
import subprocess
import numpy as np
import time
import psycopg2



def main(argv):
    scale = ""
    database_name = ''
    try:
        opts, args = getopt.getopt(argv,"hD:s:",["Database=","scale="])
    except getopt.GetoptError:
        print("DemowPINQ -D <Databse name> -s <scale:0.01/0.05/.../10>")
        sys.exit(2)
    for opt, arg in opts:
        if opt == '-h':
            print("DemowPINQ -D <Databse name> -s <scale:0.01/0.05/.../10>")
            sys.exit()
        elif opt in ("-D","--Database"):
            database_name = arg
        elif opt in ("-s", "--scale"):
            scale = arg
    if scale not in ("0.01","0.05","0.1","0.5","1","5","10"):
        print("Invalid scale, please input 0.01/0.05/0.1/0.5/1/5/10")
        sys.exit()
            
    cur_path=os.getcwd()
    cmd = "python "+cur_path+"/ImportDataToDB.py -d TPCH -s "+scale+" -D "+database_name
    subprocess.run(cmd, shell=True)
    cmd = "python "+cur_path+"/ImportDataToDB.py -d Facebook -D "+database_name
    subprocess.run(cmd, shell=True)
    con = psycopg2.connect(database=database_name)
    cur = con.cursor()
    for i in range(8):
        query_id = str(i+1)
        query_path = cur_path+"/../wPINQ/Q"+query_id+".txt"
        origin_query_path = cur_path+"/../query/query_result/Q"+query_id+".sh"
        queries = open(query_path,'r')
        origin_queries = open(origin_query_path,'r')
        query= ""
        ans = 0
        for line in queries.readlines():
            query = query+line
            if ";" in query:
                query = query.replace('\n'," ")
                cur.execute(query)
                if "create" in query:
                    query= ""
                    continue;
                if "Create" in query:
                    query= ""
                    continue; 
                if "CREATE" in query:
                    query= ""
                    continue;  
                query= ""
                ans = cur.fetchone()
        ans = float(ans[0])
        query= ""
        res = 0
        for line in origin_queries.readlines():
            query = query+line
            if ";" in query:
                query = query.replace('\n'," ")
                cur.execute(query)
                if "create" in query:
                    query= ""
                    continue;
                if "Create" in query:
                    query= ""
                    continue; 
                if "CREATE" in query:
                    query= ""
                    continue;
                if "drop" in query:
                    query= ""
                    continue;
                if "Drop" in query:
                    query= ""
                    continue; 
                if "DROP" in query:
                    query= ""
                    continue;
                query= ""
                res = cur.fetchone()
                res = int(res[0])
        print("For query "+str(query_id))
        print("The output of wPINQ is "+str(ans))
        print("The output of exact query "+str(res))
        print("The ratio between them is "+str(ans/res))
        clean_query_path = cur_path+"/../wPINQ/Q"+query_id+"_drop.txt"
        clean_queries = open(clean_query_path,'r')
        query= ""
        for line in clean_queries.readlines():
            query = query+line
            if ";" in query:
                query = query.replace('\n'," ")
                cur.execute(query)
                query= ""

    con.commit()
    con.close()
    cmd = "python "+cur_path+"/ExportDataFromDB.py -d Facebook -D "+database_name
    subprocess.run(cmd, shell=True)
    cmd = "python "+cur_path+"/ExportDataFromDB.py -d TPCH -D "+database_name
    subprocess.run(cmd, shell=True)
        
    
    
if __name__ == "__main__":
   main(sys.argv[1:])
