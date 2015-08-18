# This is an auto-generated Django model module.
# You'll have to do the following manually to clean this up:
#   * Rearrange models' order
#   * Make sure each model has one field with primary_key=True
#   * Remove `managed = False` lines if you wish to allow Django to create, modify, and delete the table
# Feel free to rename the models, but don't rename db_table values or field names.
#
# Also note: You'll have to insert the output of 'django-admin sqlcustom [app_label]'
# into your database.
from __future__ import unicode_literals

from django.db import models

class Activitymodel(models.Model):
    activityid = models.IntegerField(db_column='activityID', primary_key=True)  # Field name made lowercase.
    begin = models.DateTimeField(blank=True, null=True)
    end = models.DateTimeField(blank=True, null=True)
    parms = models.CharField(max_length=255, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'activitymodel'
        
    def __str__(self):
        return "活动id:"+str(self.activityid)

class Blacklistdata(models.Model):
    userid = models.IntegerField(db_column='UserID', primary_key=True)  # Field name made lowercase.
    username = models.CharField(db_column='UserName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    score = models.IntegerField(db_column='Score', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'blacklistdata'
        
    def __str__(self):
        return self.username


class Gameuser(models.Model):
    userid = models.IntegerField(db_column='UserId', primary_key=True)  # Field name made lowercase.
    nickname = models.CharField(db_column='NickName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    passportid = models.CharField(db_column='PassportId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    retailid = models.CharField(db_column='RetailId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.
    score = models.IntegerField(db_column='Score', blank=True, null=True)  # Field name made lowercase.
    createtime = models.DateTimeField(db_column='CreateTime', blank=True, null=True)  # Field name made lowercase.
    version = models.CharField(max_length=255, blank=True, null=True)
    state = models.IntegerField(blank=True, null=True)
    diamond = models.IntegerField(db_column='Diamond', blank=True, null=True)  # Field name made lowercase.
    thetotal = models.IntegerField(db_column='theTotal', blank=True, null=True)  # Field name made lowercase.
    preranking = models.IntegerField(db_column='preRanking', blank=True, null=True)  # Field name made lowercase.
    compensationdate = models.DateTimeField(db_column='CompensationDate', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'gameuser'
        
    def __str__(self):
        return self.nickname


class Happymodedata(models.Model):
    the3rduserid = models.IntegerField(db_column='the3rdUserId', primary_key=True)  # Field name made lowercase.
    enternum = models.IntegerField(db_column='EnterNum', blank=True, null=True)  # Field name made lowercase.
    happypoint = models.IntegerField(db_column='HappyPoint', blank=True, null=True)  # Field name made lowercase.
    happyrelivenum = models.IntegerField(db_column='HappyReliveNum', blank=True, null=True)  # Field name made lowercase.
    realitembuycntinrefleshtime = models.TextField(db_column='realItemBuyCntInRefleshTime', blank=True, null=True)  # Field name made lowercase.
    prerefleshtime = models.DateTimeField(db_column='PreRefleshTime', blank=True, null=True)  # Field name made lowercase.
    actionenterhappypoint = models.TextField(db_column='ActionEnterHappyPoint', blank=True, null=True)  # Field name made lowercase.
    realiteminfolst = models.TextField(db_column='RealItemInfoLst', blank=True, null=True)  # Field name made lowercase.
    payinfodic = models.TextField(db_column='PayInfoDic', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'happymodedata'
        
    def __str__(self):
        return str(self.the3rduserid)


class Userranking(models.Model):
    userid = models.IntegerField(db_column='UserID', primary_key=True)  # Field name made lowercase.
    username = models.CharField(db_column='UserName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    score = models.IntegerField(db_column='Score', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'userranking'
        
    def __str__(self):
        return str(self.username)
