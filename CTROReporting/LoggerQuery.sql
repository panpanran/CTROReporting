select * from Loggers where 
MethodName = 'TSRFeedback'
--and DATEPART(year, CreatedDate) = 2018 
--and DATEPART(month, CreatedDate) = 12
--and DATEPART(day, CreatedDate) = 21
and Message like '%Error%';
