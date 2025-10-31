#/bin/bash

declare -i a
declare -i b
declare -i c
declare name
declare Path
declare typed
declare typex
declare -i port
Path='LogisticsDelivery'
typed='Web'
port=8060

name=${Path,,}
typex=${typed,,}


file="build.txt"
a=$(cat $file)
b=0
if [ $a == $b ]
then
 echo "文件burld.txt不存在"
else
  echo "执行删除docker 操作 $a"
fi
b=a+1
docker build -t mychy/${name}${typex}:${b} -f ./Dockerfile${typed} .
if [ $a>0 ]
then
	docker stop ${Path}${typed}$a
	docker update --restart=no ${Path}${typed}${a}
fi
docker run  --restart=always --privileged=true -p ${port}:5000 -v /alidata/docker/logs/${Path}.${typed}:/webcore/AppData -v /alidata/docker/config/${Path}.${typed}:/webcore/config  -v /alidata/docker/shard/${Path}.${typed}/Files:/webcore/wwwroot/Files   --name ${Path}${typed}$b -d mychy/${name}${typex}:$b
c=a-2
if [ $c>0 ]
then
	docker rm ${Path}${typed}$c
	docker rmi mychy/${name}${typex}:$c
fi
docker ps
echo "$b" > $file