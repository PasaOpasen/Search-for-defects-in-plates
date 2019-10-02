
#https://stepik.org/lesson/26186/step/3?unit=8128
smart_test <-  function(x){
  tab=table(x)
  if(sum(tab[tab<5])>0){
    return(fisher.test(tab)$p.value)
  }else{
    tmp=chisq.test(tab )
    return(c(tmp$statistic,tmp$parameter,tmp$p.value))
  }
  
}
# Достаточно наблюдений в таблице
table(mtcars[,c("am", "vs")])
 smart_test(mtcars[,c("am", "vs")])
 
 
 
 
 test_data <- read.csv("https://stepic.org/media/attachments/course/524/test_data.csv", stringsAsFactors = F)
 
 str(test_data)
 
 most_significant <-  function(x){
   t=sapply(x, function(y) chisq.test(c(sum(y =="A"),sum(y =="T"),sum(y =="G"),sum(y =="C")))$p.value)
   return( colnames(x)[which(t==min(t))] )
 }
 
 most_significant(test_data)
 
 test_data$V1=="A"
 y=test_data$V2
chisq.test(c(sum(y =="A"),sum(y =="T"),sum(y =="G"),sum(y =="C")))$p.value
 
 



gen=sapply(iris[,1:4], mean)
res=apply(iris[,1:4], 1, function(x) ifelse(sum(x>gen)>=3,1,0))
iris$important_cases=factor(res,labels = c("No","Yes"))
str(iris$important_cases)
table(iris$important_cases)


get_important_cases <- function(x){
  n=ncol(x)%/%2
  gen=sapply(x, mean)
  res=apply(x, 1, function(x) ifelse(sum(x>gen)>n,1,0))
  x$important_cases=factor(res,levels=c(0,1), labels = c("No","Yes"))
  return(x)
} 
test_data <- data.frame(V1 = c(16, 21, 18), 
                        V2 = c(17, 7, 16), 
                        V3 = c(25, 23, 27), 
                        V4 = c(20, 22, 18), 
                        V5 = c(16, 17, 19))

get_important_cases(test_data)



stat_mode <- function(x){
  x.t<-table(x)
 return(sort(unique(x))[which(x.t==max(x.t))] ) 
}
v <- c(1, 2, 3, 3, 3, 4, 5)
v <- c(10,11,9,3,5,9,1,2,14,8,11,18,8,6,2,2)
v <- c(17,20,12,9,9,14,11,11,13,15,1,3,16)
stat_mode(v)


max_resid <- function(x){
  d <-  chisq.test(table(x))$stdres
  print(d)
 ind <- which(d==max(d), arr.ind = T)         
  row_names <- rownames(ind)
   col_names <-  ifelse(ind[2]==1,"positive","negative")
   return(c(row_names, col_names)) 
}
test_data <- read.csv("https://stepic.org/media/attachments/course/524/test_drugs.csv")
str(test_data)
          
test_data=as.data.frame(list(Drugs = c(3, 2, 3, 2, 1, 3, 3, 2, 1, 2, 3, 2, 2, 2, 1, 2, 2, 1, 3, 2, 1, 3, 2, 2, 3, 1, 1, 2, 3, 3, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 3, 2, 2, 3, 2, 2, 3, 1, 1, 2, 2, 2, 1, 1, 2, 3, 1, 2, 2, 2, 2, 3, 2, 2, 3, 2, 2, 2, 2, 2, 1, 1, 3, 2, 2, 1, 2, 2, 2, 2, 3, 2, 3, 3, 1, 1, 1, 1, 3, 2, 2, 3, 1, 2, 2, 3, 2, 1, 3, 3, 1, 2, 3), Result = c(2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 1,1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 1, 1, 2, 2, 1, 2, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 2, 2, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)))                                                                                                                                                                                                                                                                                                                                                         
max_resid(test_data)






library(ggplot2)
obj <- ggplot(diamonds,aes(x=color,fill=cut))+
  geom_bar(position = 'dodge')
obj


log(0.3/0.7)
s=exp(-1.15-0.17+2.13+0.8)
(s/(1+s))
p=24/41
log(p/(1-p))



















 