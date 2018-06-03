  ;WITH DS_CTE (TE, InHouse)
  AS (
	select TE, InHouse from [dbo].[DataSheets] where date between '2018-03-01' and '2019-03-31' and Status = 'RUN' 
	GROUP BY TE, InHouse
  )
  select InHouse, month(u.date), count(1)
  from [OnlineEvents].[dbo].[upload] u
  join DS_CTE ds on u.[TM CODE] = ds.te 
  where convert(DATE,u.[DATE]) between '2018-01-01' and '2019-01-01'
  and u.[FIRST USE] = 'YES'
  and u.[BRAND] = 'NEPRO VANILLA'
  group by InHouse, month(u.date)