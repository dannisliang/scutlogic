from django.shortcuts import render

# Create your views here.

from django.shortcuts import render_to_response
from django.shortcuts import render
from django.template import Context
from mysite.forms import blackform
#for django form
from django.template.context_processors import csrf
# http redirect
from django.http import HttpResponseRedirect

###################################
import http.client
import urllib
import urllib.request
import urllib.error

def getGSInfo(test_data):
    try:
        test_data.encode('utf-8')
        url = 'http://182.92.156.9:8036/Service.aspx/webBackOffice'
        headerdata={'Host':"182.92.156.9"}
        conn = http.client.HTTPConnection("182.92.156.9",8036);
        conn.request(method="POST",url=url,body=test_data,headers=headerdata)
        response=conn.getresponse()
        res = response.read()
        #result = "type:"+str((res))
        if None == res :
            result = "error"
        else:
            result = res.decode('utf-8')
    except :
        result = '网络错误'
        
    return result
#####################################

def runtime(request):
    if False == request.user.is_authenticated():
        return HttpResponseRedirect('/admin')
        
    menu = {}
    menu['black'] = 'black'
    menu['addHappyPoint'] = 'http://www.google.com'
    return render_to_response('runtime/cmd.html',{'dic':menu})
    
def blackInfo():
    preCmd = 'blackInfo:10' 
    preInfo = getGSInfo(preCmd)
    preList =  preInfo.split(',')
    return preList
    
def black(request):
    if False == request.user.is_authenticated():
        return HttpResponseRedirect('/admin')
    c = {}
    c.update(csrf(request))
    
    if request.method == 'POST':
        form = blackform(request.POST)
        if form.is_valid():
            cd = form.cleaned_data
            cmd = 'black:'+str(form.cleaned_data['id'])
            ret = getGSInfo(cmd)
            c['notify'] = "执行成功,查看pre,after寻找结果"
            c['after'] = ret
            c['pre'] = blackInfo()
    else:
        c['pre'] = blackInfo()
        c['notify'] = '请输入要删除的玩家在排行榜上的名次.'
        form = blackform(
            initial={'message':'删除原因: 作弊'}
        )
        
    c['form'] = form
    return render_to_response('runtime/black.html',c)
    
