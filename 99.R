
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




get_coefficients <- function(dataset){
  
  mod=glm(y~x,dataset,family="binomial")
  
  return(exp(mod$coefficients))
  
}

centered <- function(test_data, var_names){
  
  library(dplyr)
  p=test_data%>%mutate_at( var_names, function(x) x-mean(x))
  
  return(p)
}
test_data <- read.csv("https://stepic.org/media/attachments/course/524/cen_data.csv")
test_data
var_names = c("X4", "X2", "X1")
centered(test_data, var_names)



get_features <- function(dataset){
  
  fit <- glm(is_prohibited ~ weight+length+width+type, dataset, family = "binomial")
  result <- anova(fit, test = "Chisq")$Pr
  p=colnames(dataset)[!is.na(result) & result<0.05]
  if(length(p)==0){
    return("Prediction makes no sense")
  }else{
    return(p)
  }
  
}
test_data <- read.csv("https://stepic.org/media/attachments/course/524/test_luggage_2.csv")
str(test_data)
get_features(test_data)



most_suspicious <- function(test_data, data_for_predict){
  fit <- glm(is_prohibited ~ ., test_data, family = "binomial")
  tmp=predict.glm(fit,newdata = data_for_predict[,1:4],type = "response")

  pr=sapply(tmp, function(x) {
    k=exp(x)
    return(k/(1+k))
  })
  
  
  return(as.vector(data_for_predict[pr==max(pr),5]))
  
}

test_data <- read.csv("https://stepic.org/media/attachments/course/524/test_data_passangers.csv")
str(test_data)
data_for_predict <-read.csv("https://stepic.org/media/attachments/course/524/predict_passangers.csv")
str(data_for_predict)
most_suspicious(test_data, data_for_predict)




normality_test <- function(dataset){
  
  num=sapply(dataset,is.numeric)
  
  vec=sapply(dataset[,num], function(x) shapiro.test(x)$p.value)
  
  return( vec)
  
}
normality_test(iris)



smart_anova <- function(test_data){
  
  t1=shapiro.test(test_data[test_data$y=="A",]$x)$p.value
  t2=shapiro.test(test_data[test_data$y=="B",]$x)$p.value
  t3=shapiro.test(test_data[test_data$y=="C",]$x)$p.value
  
  sm=sum(c(t1,t2,t3)<0.05)+sum(bartlett.test(x~y,test_data)$p.value<0.05)
  if(sm==0){
    fit <- aov(x ~ y, test_data)
    p_value <- summary(fit)[[1]]$'Pr(>F)'[1]
    res=c(p_value)
    names(res)=c("ANOVA")
    return(res)
  }else{
    p_value <- kruskal.test(x ~ y, test_data)$p.value
    res=c(p_value)
    names(res)=c("KW")
    return(res)
  }
}

test_data <- read.csv("https://stepic.org/media/attachments/course/524/s_anova_test.csv")
str(test_data)
smart_anova(test_data)
test_data <- as.data.frame(list(x = c(2.08, 0.82, -0.21, -0.74, -0.99, 0.86, 0.52, 0.39, -1.49, -0.17, -0.1, 0.08, -0.65, 1.03, 2.99, 0.09, 0.13, -0.53, 2.9, 0.38, 0.37, -1.46, -0.26, -0.35, -0.52, 0.17, 1.4, -1.16, -1.76, -1.68), y = c(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3)))
test_data$y <-  factor(test_data$y, labels = c('A', 'B', 'C'))

smart_anova(test_data)



library(dplyr)
normality_by <- function(test){

  tmp=test%>%group_by_(.dots=lapply(colnames(.)[2:3], as.symbol))%>%
    summarise_if(is.numeric,function(x)shapiro.test(x)$p.value)

  tmp=as.data.frame(tmp)
  colnames(tmp)[length(colnames(tmp))]="p_value"
  return(tmp)

}
normality_by(mtcars[, c("mpg", "am", "vs")])




library(ggplot2)
obj <- ggplot(iris,aes(x=Sepal.Length,fill=Species))+
  geom_density(alpha = 0.2) 
obj


















 