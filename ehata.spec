Name:		ehata
Version:	3.1.0.0
Release:	1%{?dist}
Summary:	Extended Hata (eHata) Urban Propagation Model

Group:		Development/Libraries
License:	public domain
URL:		https://github.com/NTIA/ehata
#git archive --format=tar.gz -o ehata-3.1.0.0.tar.gz --prefix=ehata-3.1.0.0/ master
Source0:	https://github.com/NTIA/ehata/archive/master.tar.gz?prefix=ehata-3.1.0.0;o=./ehata-3.1.0.0.tar.gz

BuildRequires:	clang
#Requires:	

%description
The Extended Hata (eHata) Urban Propagation Model - This code repository
contains a C++ reference version of the eHata urban propagation model. The model
was developed by NTIA and used in NTIA Technical Report TR-15-517, "3.5 GHz
Exclusion Zone Analyses and Methodology".

%prep
%setup -q

%build
pushd build
cmake3 -DCMAKE_INSTALL_PREFIX=/usr -DENABLE_DOCS=YES -DBUILD_SHARED_LIBS=true -DLIB_SUFFIX=64 ..
make %{?_smp_mflags}
popd

%install
pushd build
make install DESTDIR=%{buildroot}
popd


%files
%doc



%changelog

