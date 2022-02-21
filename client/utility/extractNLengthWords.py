import pymongo

dictFile = open("raw-dict.txt", "r")
words = dictFile.readlines()



myclient = pymongo.MongoClient("mongodb://127.0.0.1:27017/")
mydb = myclient["db"]
mycol = mydb["Words"]

for l in range(3, 11):
    for i in words:
        if len(i.strip()) == l:
            mydict = { 
                "word": i.strip(), 
                "length": l,
                "last_used": None 
                }
            x = mycol.insert_one(mydict)

print("done")
