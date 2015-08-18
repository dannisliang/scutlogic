from django.contrib import admin
from learn.models import Publisher,Author,Book

class AuthorAdmin(admin.ModelAdmin):
	#自定义显示
	list_display=('first_name','last_name','email')
	#自定义查询
	search_fields = ('first_name','last_name')
	
class BookAdmin(admin.ModelAdmin):
	#显示数据
	list_display = ('publisher','title','publication_date',)
	#右侧过滤器
	list_filter = ('publication_date','publisher',)
	#顶部过滤器，仅支持时间，用于时间精确过滤
	date_hierarchy = 'publication_date'
	#显示数据默认排序
	ordering = ('publication_date',)
	#编辑表单显示
	#fields= ('title','authors','publisher','publication_date',)
	#多对多显示。。且不能用于外键
	filter_horizontal=('authors',)
	#优化默认显示,下拉列表显示。文本加快加载
	raw_id_fields = ('publisher',)
	
admin.site.register(Publisher)
admin.site.register(Author,AuthorAdmin)
admin.site.register(Book,BookAdmin)
