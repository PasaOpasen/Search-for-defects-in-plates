
decorate_string <- function(pattern, ...) { 
  res=paste(...)
  pt=intToUtf8(rev(utf8ToInt(pattern)))#интересный реверс строки!
  return(paste0(pattern,res,pt))
}

decorate_string(pattern = "123", "abc")            # "123abc321"
decorate_string(pattern = "123", "abc", "def")     # "123abc def321"
decorate_string(pattern = "123", c("abc", "def"))  # "123abc321" "123def321" (вектор длины 2)  

decorate_string(pattern = "123", "abc", "def", sep = "+")    # "123abc+def321"
decorate_string(pattern = "!", c("x", "x"), collapse = "_")  # "!x_x!"
decorate_string(pattern = ".:", 1:2, 3:4, 5:6, sep = "&")    # ".:1&3&5:." ".:2&4&6:." (вектор длины 2)




generator<- function(set, prob = rep(1/length(set), length(set)))
  function(n) sample(set, n, replace=T, prob) 

roulette_values <- c("Zero!", 1:36)
roulette_values
a<-c(rep(2/(length(roulette_values)+1),1),rep(1/(length(roulette_values) +1), (length(roulette_values)-1)))

print(sum(a))
print(a) 

fair_roulette<- generator(roulette_values) 
fair_roulette(37)

rigged_roulette <- generator(roulette_values, prob=a) 
rigged_roulette(37) 
