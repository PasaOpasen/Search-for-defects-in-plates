library(data.table)
library(ggplot2)
library(plotly)
library(htmlwidgets)

strings=readLines("InterGraficPaths.txt")
files=paste0(strings,".txt")

x=fread(files[1])
if(length(x)>500){
 sequ=seq(1,length(x),by=length(x)%/%500) 
}else{
  sequ=1:length(x)
}
x=x[sequ]


for(i in 2:length(files)){
  y=fread(files[i])[sequ]
  
  df=data.frame(x,y)
  colnames(df)=c(x=strings[1],y=strings[i])
  ob=ggplotly(
  ggplot(df,aes(x=x,y=y))+geom_line()+
    theme_bw()
  )
  saveWidget(as.widget(ob),paste0(strings[i], ".html"), FALSE)
}