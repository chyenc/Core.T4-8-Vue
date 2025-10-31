FROM registry.huiyuanjuice.cn/aspnetcore-img:6.0
LABEL author=MyChy
RUN mkdir /webcore
COPY web /webcore
WORKDIR /webcore
EXPOSE 8080
CMD [ "dotnet","Qs.WebApi.dll" ]

