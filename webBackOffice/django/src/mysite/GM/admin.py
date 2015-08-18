from django.contrib import admin
from GM.models import Blacklistdata,Gameuser,Happymodedata,Userranking
from GM.models import Activitymodel
# Register your models here.
# admin.site.register(Authority)
admin.site.register(Blacklistdata)
admin.site.register(Gameuser)
admin.site.register(Happymodedata)
admin.site.register(Userranking)
admin.site.register(Activitymodel)
