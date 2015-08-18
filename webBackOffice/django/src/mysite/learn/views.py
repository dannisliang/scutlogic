from django.shortcuts import render
from django.core.urlresolvers import reverse
# Create your views here.

import http.client
import urllib
import urllib.request
import urllib.error
#
# 中文encode报错 BEGIN
# 
#import sys
#import imp
#imp.reload(sys)
#sys.setdefaultencoding('utf-8')
#
# 中文encode报错 END
#

def getGSInfo(test_data):
	test_data.encode('utf-8')
	url = 'http://127.0.0.1:8036/Service.aspx/webBackOffice'
	headerdata={'Host':"127.0.0.1"}
	conn = http.client.HTTPConnection("127.0.0.1",8036);
	conn.request(method="POST",url=url,body=test_data,headers=headerdata)
	response=conn.getresponse()
	res = response.read()
	#result = "type:"+str((res))
	if None == res :
		result = "error"
	else:
		result = res.decode('utf-8')
	return result

from django.http import HttpResponse

def index(request):
	info = reverse('home',args='') + "hi sally 元元x"
	return HttpResponse(info)
	
def home(request):
	return render(request,'home.html')
	
def base(request):
	return render(request,'base.html')
	
import json
def val(request):
	List = ['你好sally','萌萌再见x']
	Dict = {'name':'sally'}
	return render(request,'val.html',{
		'List':json.dumps(List),
		'Dict':json.dumps(Dict)
		})
		
		
class Person(object):
	def __init__(self,name,age):
		self.name = name
		self.age = age
		
import datetime
from django.template import Context
from django.shortcuts import render_to_response
def current_datetime(request):
	now = datetime.datetime.now()
	person=Person('guccang',27)
	#assert false
	return render_to_response('test.html',locals())

def hours_ahead(request,offset):
	try:
		offset = int(offset)
	except ValueError:
		raise Http404()
	#assert false	
	dt = datetime.datetime.now() + datetime.timedelta(hours=offset)
	return render_to_response('test01.html',locals())

from django import forms
class AddForm(forms.Form):
	cmd = forms.CharField(max_length=512 ,label='命令')
	parms	 =  forms.CharField(max_length=512 ,label='参数输入')
	
def form(request):
	if request.method == "POST" :
		form = AddForm(request.POST)
		
		if form.is_valid():
			cmd = form.cleaned_data['cmd']
			parms = form.cleaned_data['parms']
			sendData = str(cmd)+":"+parms
			res = getGSInfo(sendData)
			res = sendData + ":" + res
			return HttpResponse(res)
	else:
		form = AddForm()
		
	return render(request,'form.html',{'form':form})
		

	
def GMTools(request):
	values = request.META.items()
	return render(request,'GMTest.html',{'mataInfo':values})
	
	
#表单
from django.shortcuts import render_to_response
from learn.forms import ContactForm
from django.http import HttpResponseRedirect
from django.template.context_processors import csrf
#@csrf_exempt
def contact(request):
	if request.method == 'POST':
		form = ContactForm(request.POST)
		if form.is_valid():
			cd = form.cleaned_data
			return HttpResponseRedirect('/time')
	else:
		form = ContactForm(
			initial={'message':'I love your , sally'}
		)
		
	c = {}
	c.update(csrf(request))
	c['form'] = form
	return render_to_response('contact_form.html',c)
	
	

